﻿// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using LeadingCode.RedisPack.Models;
using Volo.Abp.DependencyInjection;
using Wpf.Ui.Controls.Navigation;

namespace LeadingCode.RedisPack.ViewModels;

public partial class DataViewModel : ObservableObject, INavigationAware, IScopedDependency
{
    private bool _isInitialized = false;

    [ObservableProperty]
    private IEnumerable<DataColor> _colors;

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
}
