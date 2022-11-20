// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch.Pages;

/// <summary>The Log Page contains the most recent log messages</summary>
/// <remarks>Size of message queue is set in the LogMethod method in MainWindow.xaml.cs</remarks>
public partial class LogPage : Page
{
    public LogPage()
    {
        InitializeComponent();

        // Settings change event
        UserSettings.Setting.PropertyChanged += UserSettingChanged;

        MsgQueue.MessageQueue.CollectionChanged += MessageQueue_CollectionChanged;
    }

    private void UserSettingChanged(object sender, PropertyChangedEventArgs e)
    {
        //PropertyInfo prop = sender.GetType().GetProperty(e.PropertyName);
        //object newValue = prop?.GetValue(sender, null);
        switch (e.PropertyName)
        {
            case nameof(UserSettings.Setting.DarkMode):
                RefreshListbox();
                Debug.WriteLine("xxxxxxxxxx");
                break;
        }
    }

    #region Scroll to bottom when message queue changes
            /// <summary>Handles the CollectionChanged event of the MessageQueue control.</summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
    private void MessageQueue_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        ScrollToBottom(lb1);
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

    #region Page events

    /// <summary>Handles the Loaded event of the Page control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ScrollToBottom(lb1);
    }
    #endregion Page events

    #region Button click events
    /// <summary>Handles the Click event of the BtnLogFile control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void BtnLogFile_Click(object sender, RoutedEventArgs e)
    {
        string logFile = NLHelpers.GetLogfileName();
        TextFileViewer.ViewTextFile(logFile);
    }

    /// <summary>Handles the Click event of the BtnLogFolder control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void BtnLogFolder_Click(object sender, RoutedEventArgs e)
    {
        string logFile = NLHelpers.GetLogfileName();
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

    public void RefreshListbox()
    {
        lb1.Items.Refresh();
    }
}
