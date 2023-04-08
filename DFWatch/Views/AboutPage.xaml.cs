﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch.Views;
/// <summary>
/// Interaction logic for AboutPage.xaml
/// </summary>
public partial class AboutPage : UserControl
{
    public AboutPage()
    {
        InitializeComponent();

        txtBuildDate.Text = $"{BuildInfo.BuildDateUtc:f}  (UTC)";
    }
}
