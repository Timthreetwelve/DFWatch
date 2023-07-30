// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch.ViewModels;

internal partial class NavigationViewModel : ObservableObject
{
    #region Constructor
    public NavigationViewModel()
    {
        NavigateToPage(NavPage.Main);

        Instance = this;
    }
    #endregion Constructor

    private readonly MainWindow _mainWindow = Application.Current.MainWindow as MainWindow;

    public static NavigationViewModel Instance { get; private set; }

    #region Observable collection of navigation items
    public static List<NavigationItem> NavigationViewModelTypes { get; set; } = new List<NavigationItem>
        (new List<NavigationItem>
            {
                new NavigationItem
                {
                    Name="Log",
                    NavPage = NavPage.Main,
                    ViewModelType= typeof(MainViewModel),
                    IconKind=PackIconKind.FileDocumentOutline,
                    PageTitle="Activity Log"
                },
                new NavigationItem
                {
                    Name="Settings",
                    NavPage=NavPage.Settings,
                    ViewModelType= typeof(SettingsViewModel),
                    IconKind=PackIconKind.SettingsOutline,
                    PageTitle = "Settings, Options & Preferences"
                },
                new NavigationItem
                {
                    Name="About",
                    NavPage=NavPage.About,
                    ViewModelType= typeof(AboutViewModel),
                    IconKind=PackIconKind.AboutCircleOutline,
                    PageTitle = "About Download Folder Watcher"
                },
                new NavigationItem
                {
                    Name="Exit",
                    IconKind=PackIconKind.ExitToApp,
                    IsExit=true
                }
            }
        );
    #endregion Observable collection of navigation items

    #region Properties
    [ObservableProperty]
    private object _currentViewModel;

    [ObservableProperty]
    private string _pageTitle;
    #endregion Properties

    #region Navigate Command
    [RelayCommand]
    internal void Navigate(object param)
    {
        if (param is NavigationItem item)
        {
            if (item.IsExit)
            {
                Application.Current.Shutdown();
            }
            else if (item.ViewModelType is not null)
            {
                PageTitle = item.PageTitle;
                CurrentViewModel = Activator.CreateInstance((Type)item.ViewModelType);
            }
        }
    }

    public void NavigateToPage(NavPage page)
    {
        Navigate(NavigationViewModelTypes.Find(x => x.NavPage == page));
    }
    #endregion Navigate Command

    #region Start watching
    //[RelayCommand(CanExecute = nameof(CanStartWatching))]
    [RelayCommand]
    public void StartWatching()
    {
        Watch.StartWatcher();
        //StartWatchingCommand.NotifyCanExecuteChanged();
        //StopWatchingCommand.NotifyCanExecuteChanged();
    }

    public static bool CanStartWatching()
    {
        return !Watch.Watcher.EnableRaisingEvents;
    }
    #endregion Start watching

    #region Stop watching
    //[RelayCommand(CanExecute = nameof(CanStopWatching))]
    [RelayCommand]
    public void StopWatching()
    {
        Watch.StopWatcher();
        //StopWatchingCommand.NotifyCanExecuteChanged();
        //StartWatchingCommand.NotifyCanExecuteChanged();
    }

    private static bool CanStopWatching()
    {
        return Watch.Watcher.EnableRaisingEvents;
    }
    #endregion Stop watching

    #region Show Main Window
    [RelayCommand]
    public static void ShowMainWindow()
    {
        MainWindowHelpers.ShowMainWindow();
    }
    #endregion Show Main Window

    #region Exit Application
    [RelayCommand]
    public static void ExitApplication()
    {
        App.ExplicitClose = true;
        Application.Current.Shutdown();
    }
    #endregion Exit Application

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

    #region Key down events
    /// <summary>
    /// Keyboard events
    /// </summary>
    public void KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.L && Keyboard.Modifiers == ModifierKeys.Control)
        {
            NavigateToPage(NavPage.Main);
        }
        if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
        {
            Watch.StartWatcher();
            _mainWindow.DisappearingMessage("Watcher Started");
        }
        if (e.Key == Key.P && Keyboard.Modifiers == ModifierKeys.Control)
        {
            Watch.StopWatcher();
            _mainWindow.DisappearingMessage("Watcher Stopped");
        }
        if (e.Key == Key.F1)
        {
            NavigateToPage(NavPage.About);
        }
        if (e.Key == Key.OemComma && Keyboard.Modifiers == ModifierKeys.Control)
        {
            NavigateToPage(NavPage.Settings);
        }
        if (e.Key == Key.Add && Keyboard.Modifiers == ModifierKeys.Control)
        {
            _mainWindow.EverythingLarger();
            _mainWindow.DisappearingMessage($"Size changed to: {UserSettings.Setting.UISize}");
        }
        if (e.Key == Key.Subtract && Keyboard.Modifiers == ModifierKeys.Control)
        {
            _mainWindow.EverythingSmaller();
            _mainWindow.DisappearingMessage($"Size changed to: {UserSettings.Setting.UISize}");
        }
        if (e.Key == Key.M && Keyboard.Modifiers == ModifierKeys.Control)
        {
            switch (UserSettings.Setting.UITheme)
            {
                case ThemeType.Light:
                    UserSettings.Setting.UITheme = ThemeType.Dark;
                    break;
                case ThemeType.Dark:
                    UserSettings.Setting.UITheme = ThemeType.Darker;
                    break;
                case ThemeType.Darker:
                    UserSettings.Setting.UITheme = ThemeType.System;
                    break;
                case ThemeType.System:
                    UserSettings.Setting.UITheme = ThemeType.Light;
                    break;
            }
            _mainWindow.DisappearingMessage($"Theme changed to: {UserSettings.Setting.UITheme}");
        }
    }
    #endregion Key down events
}
