﻿// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using Volo.Abp.DependencyInjection;
using Wpf.Ui.Controls.Navigation;

namespace LeadingCode.RedisPack.Views.Pages;

/// <summary>
/// Interaction logic for DashboardPage.xaml
/// </summary>
public partial class DashboardPage : INavigableView<ViewModels.DashboardViewModel>, IScopedDependency
{
    public ViewModels.DashboardViewModel ViewModel
    {
        get;
    }

    public DashboardPage(ViewModels.DashboardViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}
