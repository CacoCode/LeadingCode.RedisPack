// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Volo.Abp.DependencyInjection;
using Wpf.Ui.Common;

namespace LeadingCode.RedisPack.ViewModels;

public partial class DashboardViewModel : ObservableObject, IScopedDependency
{
    [ObservableProperty]
    private int _counter = 0;

    [ObservableProperty]
    private string _cmd1 = "pacman -Syu";

    [ObservableProperty]
    private string _cmd2 = "pacman -Sy gcc make pkg-config";

    [ObservableProperty]
    private string _cmd3 = "tar -xvf redis.tar.gz";

    [ObservableProperty] private string _cmd4 = "cd /d/docments/redistest/redis-7.0.10";

    [ObservableProperty] private string _cmd5 = "make PREFIX=/d/redis/dist install";

    [ObservableProperty] private string _cmd6 = "redis-server.exe redis.con";

    public DashboardViewModel()
    {
        
    }

    [RelayCommand]
    private void OnCounterIncrement()
    {
        Counter++;
    }

    [RelayCommand]
    private void OnCopy(string index)
    {
        switch (index)
        {
            case "1":
                Clipboard.SetText(_cmd1);
                break;
            case "2":
                Clipboard.SetText(_cmd2);
                break;
            case "3":
                Clipboard.SetText(_cmd3);
                break;
            case "4":
                Clipboard.SetText(_cmd4);
                break;
            case "5":
                Clipboard.SetText(_cmd5);
                break;
            case "6":
                Clipboard.SetText(_cmd6);
                break;
        }
    }
}
