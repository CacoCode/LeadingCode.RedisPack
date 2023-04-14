// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using Volo.Abp.DependencyInjection;
using Wpf.Ui.Controls.Navigation;

namespace LeadingCode.RedisPack.Views.Pages;

/// <summary>
/// Interaction logic for DataView.xaml
/// </summary>
public partial class DataPage : INavigableView<ViewModels.DataViewModel>, IScopedDependency
{
    public ViewModels.DataViewModel ViewModel
    {
        get;
    }

    public DataPage(ViewModels.DataViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}
