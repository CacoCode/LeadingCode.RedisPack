﻿// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using LeadingCode.RedisPack.Views.Pages;
using System;
using System.Windows;
using Wpf.Ui.Contracts;
using Wpf.Ui.Controls.Navigation;

namespace LeadingCode.RedisPack.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : INavigationWindow
{
    public ViewModels.MainWindowViewModel ViewModel
    {
        get;
    }

    public MainWindow(ViewModels.MainWindowViewModel viewModel, 
        IPageService pageService, 
        INavigationService navigationService,
        ISnackbarService snackbarService,
        IDialogService dialogService)
    {
        ViewModel = viewModel;
        DataContext = this;

        Wpf.Ui.Appearance.Watcher.Watch(this);

        InitializeComponent();
        SetPageService(pageService);

        snackbarService.SetSnackbarControl(RootSnackbar);
        dialogService.SetDialogControl(RootDialog);
        navigationService.SetNavigationControl(RootNavigation);
        RootNavigation.Loaded += (_, _) => RootNavigation.Navigate(typeof(GeneratePage));
    }

    #region INavigationWindow methods

    public INavigationView GetNavigation()
        => RootNavigation;

    public bool Navigate(Type pageType)
        => RootNavigation.Navigate(pageType);

    public void SetPageService(IPageService pageService)
        => RootNavigation.SetPageService(pageService);

    public void ShowWindow()
        => Show();

    public void CloseWindow()
        => Close();

    #endregion INavigationWindow methods

    /// <summary>
    /// Raises the closed event.
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Make sure that closing this window will begin the process of closing the application.
        Application.Current.Shutdown();
    }

    INavigationView INavigationWindow.GetNavigation()
    {
        throw new NotImplementedException();
    }

    public void SetServiceProvider(IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }
}
