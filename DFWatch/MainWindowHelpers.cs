// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;

internal static class MainWindowHelpers
{
    /// <summary>
    /// Show the main window and set it's state to normal
    /// </summary>
    public static void ShowMainWindow()
    {
        Application.Current.MainWindow.Show();
        Application.Current.MainWindow.Visibility = Visibility.Visible;
        Application.Current.MainWindow.WindowState = WindowState.Normal;
        _ = Application.Current.MainWindow.Activate();
    }
}