// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using Volo.Abp.DependencyInjection;
using Wpf.Ui.Contracts;
using Wpf.Ui.Controls.Navigation;
using Wpf.Ui.Services;

namespace LeadingCode.RedisPack.Views.Pages;

/// <summary>
/// Interaction logic for DataView.xaml
/// </summary>
public partial class GeneratePage : INavigableView<ViewModels.GenerateViewModel>, IScopedDependency
{
    public ViewModels.GenerateViewModel ViewModel
    {
        get;
    }

    public GeneratePage(ViewModels.GenerateViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}
