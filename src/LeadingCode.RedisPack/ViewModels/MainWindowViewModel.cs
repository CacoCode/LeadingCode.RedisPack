// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Volo.Abp.DependencyInjection;
using Wpf.Ui.Common;
using Wpf.Ui.Contracts;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Navigation;

namespace LeadingCode.RedisPack.ViewModels;

public partial class MainWindowViewModel : ObservableObject, IScopedDependency
{
    private bool _isInitialized = false;

    [ObservableProperty]
    private string _applicationTitle = String.Empty;

    [ObservableProperty]
    private ObservableCollection<object> _navigationItems = new();

    [ObservableProperty]
    private ObservableCollection<object> _navigationFooter = new();

    [ObservableProperty]
    private ObservableCollection<MenuItem> _trayMenuItems = new();

    public MainWindowViewModel(INavigationService navigationService)
    {
        if (!_isInitialized)
            InitializeViewModel();
    }

    private void InitializeViewModel()
    {
        ApplicationTitle = "Windows Redis包编译器";

        NavigationItems = new ObservableCollection<object>
            {
                new NavigationViewItem()
                {
                    Content = "手动生成",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                    TargetPageType = typeof(Views.Pages.DashboardPage)
                },
                new NavigationViewItem()
                {
                    Content = "自动生成",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                    TargetPageType = typeof(Views.Pages.GeneratePage)
                },
                new NavigationViewItem()
                {
                    Content = "Redis版本",
                    Icon = new SymbolIcon{Symbol = SymbolRegular.AlignSpaceEvenlyVertical20},
                    TargetPageType = typeof(Views.Pages.RedisReleasePage)
                }
            };
        var toggleThemeNavigationViewItem = new NavigationViewItem
        {
            Content = "主题切换",
            Icon = new SymbolIcon { Symbol = SymbolRegular.PaintBrush24 }
        };
        toggleThemeNavigationViewItem.Click += OnToggleThemeClicked;
        NavigationFooter = new ObservableCollection<object>
            {

                new NavigationViewItem()
                {
                    Content = "系统设置",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                    TargetPageType = typeof(Views.Pages.SettingsPage)
                }
            };
        NavigationFooter.AddFirst(toggleThemeNavigationViewItem);
        TrayMenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem
                {
                    Header = "Home",
                    Tag = "tray_home"
                }
            };

        _isInitialized = true;
    }

    private void OnToggleThemeClicked(object sender, RoutedEventArgs e)
    {
        var currentTheme = Wpf.Ui.Appearance.Theme.GetAppTheme();

        Wpf.Ui.Appearance.Theme.Apply(currentTheme == Wpf.Ui.Appearance.ThemeType.Light ? Wpf.Ui.Appearance.ThemeType.Dark : Wpf.Ui.Appearance.ThemeType.Light);
    }
}
