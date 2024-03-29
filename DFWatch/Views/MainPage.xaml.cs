﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch.Views;

/// <summary>
/// Interaction logic for MainPage.xaml
/// </summary>
public partial class MainPage : UserControl
{
    public MainPage()
    {
        InitializeComponent();

        UserSettings.Setting.PropertyChanged += UserSettingChanged;

        MsgQueue.MessageQueue.CollectionChanged += MessageQueue_CollectionChanged;

        lbMessages.SelectedItem = null;
    }

    #region Button click events
    /// <summary>Handles the Click event of the BtnLogFile control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void BtnLogFile_Click(object sender, RoutedEventArgs e)
    {
        string logFile = NLogHelpers.GetLogfileName();
        TextFileViewer.ViewTextFile(logFile);
    }

    /// <summary>Handles the Click event of the BtnLogFolder control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void BtnLogFolder_Click(object sender, RoutedEventArgs e)
    {
        string logFile = NLogHelpers.GetLogfileName();
        string folder = Path.GetDirectoryName(logFile);
        _ = Process.Start("explorer.exe", folder);
    }

    /// <summary>Handles the Click event of the BtnCheckNow control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void BtnCheckNow_Click(object sender, RoutedEventArgs e)
    {
        Watch.CheckOnDemand();
    }

    /// <summary>Handles the Click event of the BtnClear control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void BtnClear_Click(object sender, RoutedEventArgs e)
    {
        MsgQueue.MessageQueue.Clear();
        (Application.Current.MainWindow as MainWindow)?.DisappearingMessage("Log display cleared");
    }
    #endregion Button click events

    #region Refresh message ListBox
    /// <summary>Refreshes the listbox.</summary>
    public void RefreshListbox()
    {
        lbMessages.Items.Refresh();
    }
    #endregion Refresh message ListBox

    #region Loaded event
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        ScrollToBottom(lbMessages);
    }
    #endregion Loaded event

    #region Scroll to bottom when message queue changes
    /// <summary>Handles the CollectionChanged event of the MessageQueue control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
    private void MessageQueue_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        ScrollToBottom(lbMessages);
    }

    /// <summary>Scrolls to most recently added item in the ListBox</summary>
    /// <param name="box">The ListBox</param>
    private static void ScrollToBottom(ListBox box)
    {
        if (box?.Items.Count > 1 && box.IsLoaded)
        {
            // https://learn.microsoft.com/en-us/dotnet/csharp/tutorials/ranges-indexes
            box.ScrollIntoView(box.Items[^1]);
        }
    }
    #endregion Scroll to bottom when message queue changes

    #region Listen for setting change
    /// <summary>Setting changed event.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
    private void UserSettingChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            // Refresh listbox if theme has changed so that messages will update to correct color
            case nameof(UserSettings.Setting.UITheme):
                RefreshListbox();
                break;
        }
    }
    #endregion Listen for setting change
}
