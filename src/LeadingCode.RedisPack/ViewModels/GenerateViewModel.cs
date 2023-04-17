// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeadingCode.RedisPack.Apis;
using LeadingCode.RedisPack.Models;
using Volo.Abp.DependencyInjection;
using Wpf.Ui.Contracts;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Navigation;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Wpf.Ui.Common;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Windows.Threading;
using LeadingCode.RedisPack.Helpers;
using LeadingCode.RedisPack.Services;
using Microsoft.Extensions.Configuration;

namespace LeadingCode.RedisPack.ViewModels;

public partial class GenerateViewModel : ObservableObject, INavigationAware, IScopedDependency
{
    private bool _isInitialized = false;
    private readonly IGithubRedisApi _githubRedisApi;
    private readonly IGithubRedisFileApi _githubRedisFileApi;
    private readonly IDialogService _dialogService;
    private readonly ISnackbarService _snackbarService;
    private readonly IConfiguration _configuration;
    private readonly IWritableOptions<MsysConfig> _appConfig;
    DispatcherTimer _timer;
    private GenerateState _state = GenerateState.INIT;
    private string _tempDir = string.Empty;
    private string _redisDistDir = string.Empty;
    private string _redisUnzipDir = string.Empty;
    private IDialogControl _rootDialog;

    [ObservableProperty]
    private IEnumerable<DataColor> _colors;

    [ObservableProperty] private RedisReleaseInfo? _redisReleaseInfo;

    [ObservableProperty]
    private IEnumerable<RedisReleaseInfo> _redisReleaseInfos;

    [ObservableProperty]
    private string _redisTagName;

    [ObservableProperty]
    private string _msysDir = string.Empty;

    [ObservableProperty]
    private string _redisDir = string.Empty;

    [ObservableProperty]
    private bool _isInit = false;

    public GenerateViewModel(IGithubRedisApi githubRedisApi, IDialogService dialogService, ISnackbarService snackbarService, IGithubRedisFileApi githubRedisFileApi, IConfiguration configuration, IWritableOptions<MsysConfig> appConfig)
    {
        _githubRedisApi = githubRedisApi;
        _dialogService = dialogService;
        _snackbarService = snackbarService;
        _githubRedisFileApi = githubRedisFileApi;
        _configuration = configuration;
        _appConfig = appConfig;
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
    }

    public void OnNavigatedFrom()
    {
    }

    private void InitializeViewModel()
    {
        IsInit = _appConfig.Value.IsInit;
        if (!_appConfig.Value.IsInit) MsysDir = _appConfig.Value.BaseDir;
        _rootDialog = _dialogService.GetDialogControl();

        _rootDialog.DialogHeight = 160;
        _rootDialog.Footer = new TextBlock
        {
            Margin = new Thickness(0, 10, 0, 10),
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.WrapWithOverflow,
            Text = "加载中...",
        };

        _rootDialog.Content = new ProgressRing
        {
            IsIndeterminate = true,
            VerticalAlignment = VerticalAlignment.Center,
        };

        _isInitialized = true;
    }

    [RelayCommand]
    private async Task OnGetRedisTags()
    {

        _rootDialog.Show();
        var list = await _githubRedisApi.GetAsync();
        RedisReleaseInfos = list.OrderByDescending(a => a.Name).ToList();
        _rootDialog.Hide();
    }

    [RelayCommand]
    private void OnOpenDir(string type)
    {
        var folderBrowserDialog = new FolderBrowserDialog();
        folderBrowserDialog.ShowDialog();
        switch (type)
        {
            case "msys":
                MsysDir = folderBrowserDialog.SelectedPath;
                break;
            case "save":
                RedisDir = folderBrowserDialog.SelectedPath;
                break;
        }
    }

    [RelayCommand]
    private void OnGenerate()
    {
        _rootDialog.Show();
        _tempDir = Path.Combine(RedisDir, "Temp");
        CopyDlfcn();
        DownloadRedis();
        UnZipRedisFile();
    }

    [RelayCommand]
    private void OnUpdateMSYSData()
    {
        _rootDialog.Show();
        ExecuteCmd("pacman -Syu");
    }

    [RelayCommand]
    private void OnAddPkg()
    {
        _rootDialog.Show();
        ExecuteCmd("echo y | pacman -Sy gcc make pkg-config");
    }

    private void ExecuteCmd(string arguments, string? workdir = null)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(MsysDir, "msys2.exe"),
            Arguments = arguments,
            WorkingDirectory = workdir ?? MsysDir,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            Verb = "RunAs"
        };
        var process = new Process();
        process.StartInfo = startInfo;
        process.Start();
        process.StandardInput.AutoFlush = true;
        process.WaitForExit();
        process.Close();
        Thread.Sleep(500);
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += SyuTimerTick;
        _timer.Start();
    }

    void SyuTimerTick(object sender, EventArgs e)
    {
        var isClosed = Process.GetProcessesByName("mintty").Length == 0;
        if (!isClosed) return;
        _timer.Stop();
        switch (_state)
        {
            case GenerateState.INIT:
                _rootDialog.Hide();
                _snackbarService.Show("配置成功", $"MSYS配置成功。", SymbolRegular.Checkmark12, ControlAppearance.Success);
                _appConfig.Update(opt =>
                {
                    opt.IsInit = false;
                    opt.BaseDir = MsysDir;
                });
                break;
            case GenerateState.REDIS_UNZIP_END:
                CompileRedis();
                break;
            case GenerateState.REDIS_COMPILE_END:
                CopyOtherRedisFile();
                _rootDialog.Hide();
                _snackbarService.Show("编译成功", $"文件已保存在{_redisDistDir}中，请查阅。", SymbolRegular.Checkmark12, ControlAppearance.Success);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void DownloadRedis()
    {
        var response = _githubRedisFileApi.DownloadAsync($"{RedisTagName}.tar.gz").Result;
        if (!Directory.Exists(_tempDir)) Directory.CreateDirectory(_tempDir);
        var filePath = Path.Combine(_tempDir, $"{RedisTagName}.tar.gz");
        using var fileStream = File.OpenWrite(filePath);
        response.SaveAsAsync(fileStream).Wait();
    }

    void UnZipRedisFile()
    {
        _state = GenerateState.REDIS_UNZIP_END;
        var cmd = $"tar -xvf {RedisTagName}.tar.gz";
        ExecuteCmd(cmd, _tempDir);
    }

    void CopyDlfcn()
    {
        var source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dlfcn.h");
        var target = Path.Combine(MsysDir, "usr", "include", "dlfcn.h");
        File.Copy(source, target, true);
    }

    void CompileRedis()
    {
        _state = GenerateState.REDIS_COMPILE_END;
        _redisDistDir = Path.Combine(RedisDir, $"dist-{RedisTagName}");
        var cmd = $"make PREFIX={_redisDistDir.ToUnixPath()} install";
        _redisUnzipDir = Path.Combine(_tempDir, $"redis-{RedisTagName}");
        ExecuteCmd(cmd, _redisUnzipDir);
    }

    void CopyOtherRedisFile()
    {
        //msys-2.0.dll
        var msysSource = Path.Combine(MsysDir, "usr", "bin", "msys-2.0.dll");
        var msysTarget = Path.Combine(_redisDistDir, "bin", "msys-2.0.dll");
        File.Copy(msysSource, msysTarget, true);

        //redis.conf
        var redisConfSource = Path.Combine(_redisUnzipDir, "redis.conf");
        var redisConfTarget = Path.Combine(_redisDistDir, "bin", "redis.conf");
        File.Copy(redisConfSource, redisConfTarget, true);

        //sentinel.conf
        var sentinelConfSource = Path.Combine(_redisUnzipDir, "sentinel.conf");
        var sentinelConfTarget = Path.Combine(_redisDistDir, "bin", "sentinel.conf");
        File.Copy(sentinelConfSource, sentinelConfTarget, true);
    }
}
