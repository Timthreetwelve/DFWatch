// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch.Pages;

/// <summary>The Log Page contains the most recent log messages</summary>
public partial class LogPage : Page
{
    public LogPage()
    {
        InitializeComponent();

        MsgQueue.MessageQueue.CollectionChanged += MessageQueue_CollectionChanged;
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

    /// <summary>Handles the Click event of the BtnColorMsgToggle control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void BtnColorMsgToggle_Click(object sender, RoutedEventArgs e)
    {
        (Application.Current.MainWindow as MainWindow)?.NavigateToPage(NavPage.Logs);
    }
    #endregion Button click events
}
