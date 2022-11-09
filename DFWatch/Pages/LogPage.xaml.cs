// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch.Pages;

/// <summary>
/// Interaction logic for LogPage.xaml
/// </summary>
public partial class LogPage : Page
{
    public LogPage()
    {
        InitializeComponent();

        MsgQueue.MessageQueue.CollectionChanged += MessageQueue_CollectionChanged;
    }

    #region Scroll to bottom when message queue changes
    private void MessageQueue_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        ScrollToBottom(lb1);
    }

    private static void ScrollToBottom(ListBox box)
    {
        if (box.Items.Count > 1)
        {
            Border border = (Border)VisualTreeHelper.GetChild(box, 0);
            ScrollViewer viewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
            viewer.ScrollToBottom();
        }
    }
    #endregion Scroll to bottom when message queue changes

    #region Page events
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ScrollToBottom(lb1);
    }
    #endregion Page events

    #region Button click events
    private void BtnLogFile_Click(object sender, RoutedEventArgs e)
    {
        string logFile = NLHelpers.GetLogfileName();
        TextFileViewer.ViewTextFile(logFile);
    }

    private void BtnLogFolder_Click(object sender, RoutedEventArgs e)
    {
        string logFile = NLHelpers.GetLogfileName();
        string folder = Path.GetDirectoryName(logFile);
        _ = Process.Start("explorer.exe", folder);
    }

    private void BtnColorMsgToggle_Click(object sender, RoutedEventArgs e)
    {
        (Application.Current.MainWindow as MainWindow)?.NavigateToPage(NavPage.Logs);
    }
    #endregion Button click events
}
