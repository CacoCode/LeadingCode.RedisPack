// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeadingCode.RedisPack.Apis;
using LeadingCode.RedisPack.Models;
using Volo.Abp.DependencyInjection;
using Wpf.Ui.Contracts;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Navigation;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Wpf.Ui.Common;
using System.Diagnostics;
using System.Linq;
using MessageBox = System.Windows.MessageBox;
using Wpf.Ui.Appearance;
using System.Windows.Threading;
using System.Timers;

namespace LeadingCode.RedisPack.ViewModels;

public partial class GenerateViewModel : ObservableObject, INavigationAware, IScopedDependency
{
    private bool _isInitialized = false;
    private readonly IGithubRedisApi _githubRedisApi;
    private readonly IDialogService _dialogService;
    private readonly ISnackbarService _snackbarService;
    DispatcherTimer _timer;
    private int _flag = 1;


    [ObservableProperty]
    private IEnumerable<DataColor> _colors;

    [ObservableProperty] private RedisReleaseInfo? _redisReleaseInfo;

    [ObservableProperty]
    private IEnumerable<RedisReleaseInfo> _redisReleaseInfos;

    [ObservableProperty]
    private string _msysDir = string.Empty;

    [ObservableProperty]
    private string _saveDir = string.Empty;



    public GenerateViewModel(IGithubRedisApi githubRedisApi, IDialogService dialogService, ISnackbarService snackbarService)
    {
        _githubRedisApi = githubRedisApi;
        _dialogService = dialogService;
        _snackbarService = snackbarService;
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
        var random = new Random();
        var colorCollection = new List<DataColor>();

        for (int i = 0; i < 8192; i++)
            colorCollection.Add(new DataColor
            {
                Color = new SolidColorBrush(Color.FromArgb(
                    (byte)200,
                    (byte)random.Next(0, 250),
                    (byte)random.Next(0, 250),
                    (byte)random.Next(0, 250)))
            });

        Colors = colorCollection;

        _isInitialized = true;
    }

    [RelayCommand]
    private async Task OnGetRedisTags()
    {
        var rootDialog = _dialogService.GetDialogControl();

        rootDialog.DialogHeight = 160;
        rootDialog.Footer = new TextBlock
        {
            Margin = new Thickness(0, 10, 0, 10),
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.WrapWithOverflow,
            Text = "加载中...",
        };

        rootDialog.Content = new ProgressRing
        {
            IsIndeterminate = true,
            VerticalAlignment = VerticalAlignment.Center,
        };
        rootDialog.Show();
        RedisReleaseInfos = await _githubRedisApi.GetAsync();
        rootDialog.Hide();
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
                SaveDir = folderBrowserDialog.SelectedPath;
                break;
        }
    }

    [RelayCommand]
    private void OnGenerate()
    {
        //if (MsysDir.IsNullOrWhiteSpace() || !Directory.Exists(MsysDir)
        //|| SaveDir.IsNullOrWhiteSpace() || !Directory.Exists(SaveDir)
        //|| RedisReleaseInfo == null)
        //    _snackbarService.Show("配置信息缺失", "请按规定正确填写配置信息", SymbolRegular.Fluent24, ControlAppearance.Danger);
        //配置msys环境
        ExecuteCmd("pacman -Syu");

    }

    private void ExecuteCmd(string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(MsysDir, "msys2.exe"),
            Arguments = arguments,
            WorkingDirectory = MsysDir,
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
        switch (_flag)
        {
            case 1:
                ExecuteCmd("echo y | pacman -Sy gcc make pkg-config");
                _flag++;
                break;
            case 2:
                //下载redis
                //copy文件与要修改的那一个文件
                //编译redis
                _flag++;
                break;
            case 3:
                //copy剩余三个文件
                _flag++;
                break;
        }
    }
}
