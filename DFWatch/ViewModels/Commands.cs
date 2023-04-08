// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch.ViewModels;

/// <summary>
/// Relay & RoutedUI commands
/// </summary>
public partial class Commands
{
    #region View log file
    [RelayCommand]
    public static void ViewLogFile()
    {
        TextFileViewer.ViewTextFile(NLogHelpers.GetLogfileName());
    }
    #endregion View log file

    #region View readme file
    [RelayCommand]
    public static void ViewReadMeFile()
    {
        TextFileViewer.ViewTextFile(Path.Combine(AppInfo.AppDirectory, "readme.txt"));
    }
    #endregion View readme file

    #region Exit Application
    [RelayCommand]
    public static void ExitApplication()
    {
        Application.Current.Shutdown();
    }
    #endregion Exit Application

    #region Show Main Window
    [RelayCommand]
    public static void ShowMainWindow()
    {
        MainWindowHelpers.ShowMainWindow();
    }
    #endregion Show Main Window

    #region Start & stop Watcher
    public static RoutedUICommand StopWatching { get; } =
        new("Sto_p Watcher", "StopWatching", typeof(Commands));

    public static RoutedUICommand StartWatching { get; } =
        new("Star_t Watcher", "StartWatching", typeof(Commands));
    #endregion Start & stop Watcher
}
