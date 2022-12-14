// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MaterialWindow
{
    #region Stopwatch
    private readonly Stopwatch _stopwatch = new();
    #endregion Stopwatch

    #region NLog Instance
    private static readonly Logger _log = LogManager.GetLogger("logTemp");
    #endregion NLog Instance

    #region Private fields
    private static SolidColorBrush _titleBrush;
    private readonly DispatcherTimer _msgTimer = new();
    #endregion Private fields

    public MainWindow()
    {
        SingleInstance.Create(AppInfo.AppName);

        InitializeSettings();

        InitializeComponent();

        ReadSettings();

        StartUpChecks();
    }

    #region Settings
    /// <summary>
    /// Read and apply settings
    /// </summary>
    private void InitializeSettings()
    {
        _stopwatch.Start();

        UserSettings.Init(UserSettings._appFolder, UserSettings._defaultFilename, true);
    }

    public void ReadSettings()
    {
        // Set NLog configuration
        NLHelpers.NLogConfig();

        // Unhandled exception handler
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        //Set theme
        SetBaseTheme((ThemeType)UserSettings.Setting.DarkMode);

        // Set primary accent color
        SetPrimaryColor((AccentColor)UserSettings.Setting.PrimaryColor);

        // UI size
        double size = UIScale((MySize)UserSettings.Setting.UISize);
        MainGrid.LayoutTransform = new ScaleTransform(size, size);

        // Put version number in window title
        WindowTitleVersionAdmin();

        // Log the version, build date and commit id
        _log.Info($"{AppInfo.AppName} ({AppInfo.AppProduct}) {AppInfo.AppVersion} is starting up");
        _log.Info($"{AppInfo.AppName} {AppInfo.AppCopyright}");
        _log.Debug($"{AppInfo.AppName} Build date: {BuildInfo.BuildDateUtc.ToUniversalTime():g} (UTC)");
        _log.Debug($"{AppInfo.AppName} Commit ID: {BuildInfo.CommitIDString} ");

        // Log the .NET version, app framework and OS platform
        string version = Environment.Version.ToString();
        _log.Debug($".NET version: {AppInfo.RuntimeVersion.Replace(".NET", "")}  ({version})");
        _log.Debug(AppInfo.Framework);
        _log.Debug(AppInfo.OsPlatform);
        tbIcon.ToolTipText = $"DFWatch {AppInfo.TitleVersion} - Stopped";

        // Window position
        UserSettings.Setting.SetWindowPos();
        Topmost = UserSettings.Setting.KeepOnTop;

        //Start with main page
        NavigateToPage(NavPage.Logs);

        // Minimize to tray
        tbIcon.Visibility = UserSettings.Setting.MinimizeToTray ? Visibility.Visible : Visibility.Collapsed;

        // Start minimized
        if (UserSettings.Setting.StartMinimized)
        {
            WindowState = WindowState.Minimized;
            if (UserSettings.Setting.MinimizeToTray)
            {
                Hide();
            }
        }

        // Settings change event
        UserSettings.Setting.PropertyChanged += UserSettingChanged;

        // Watch for errors
        Watch.Watcher.Error += Watcher_Error;

        // Session ending
        Application.Current.SessionEnding += Current_SessionEnding;
    }
    #endregion Settings

    #region Setting change
    /// <summary>
    /// My way of handling changes in UserSettings
    /// </summary>
    /// <param name="sender"></param>
    private void UserSettingChanged(object sender, PropertyChangedEventArgs e)
    {
        PropertyInfo prop = sender.GetType().GetProperty(e.PropertyName);
        object newValue = prop?.GetValue(sender, null);
        _log.Debug($"Setting change: {e.PropertyName} New Value: {newValue}");
        switch (e.PropertyName)
        {
            case nameof(UserSettings.Setting.KeepOnTop):
                Topmost = (bool)newValue;
                break;

            case nameof(UserSettings.Setting.IncludeDebugInFile):
                NLHelpers.SetLogToFileLevel((bool)newValue);
                break;

            case nameof(UserSettings.Setting.IncludeDebugInGui):
                NLHelpers.SetLogToMethodLevel((bool)newValue);
                break;

            case nameof(UserSettings.Setting.DarkMode):
                SetBaseTheme((ThemeType)newValue);
                break;

            case nameof(UserSettings.Setting.PrimaryColor):
                SetPrimaryColor((AccentColor)newValue);
                break;

            case nameof(UserSettings.Setting.UISize):
                int size = (int)newValue;
                double newSize = UIScale((MySize)size);
                MainGrid.LayoutTransform = new ScaleTransform(newSize, newSize);
                break;

            case nameof(UserSettings.Setting.MinimizeToTray):
                tbIcon.Visibility = (bool)newValue ? Visibility.Visible : Visibility.Collapsed;
                break;

            case nameof(UserSettings.Setting.StartWithWindows):
                if ((bool)newValue)
                {
                    AddStartToRegistry();
                }
                else
                {
                    RemoveStartFromRegistry();
                }
                break;

            case nameof(UserSettings.Setting.Heartbeat):
                if ((bool)newValue)
                {
                    Heartbeat.StartHeartbeat();
                }
                else
                {
                    Heartbeat.StopHeartbeat();
                }
                break;
        }
    }
    #endregion Setting change

    #region Set light, dark or darker theme
    /// <summary>
    /// Gets the current theme
    /// </summary>
    /// <returns>Dark or Light</returns>
    internal static string GetSystemTheme()
    {
        BaseTheme? sysTheme = Theme.GetSystemTheme();
        return sysTheme != null ? sysTheme.ToString() : string.Empty;
    }

    /// <summary>
    /// Sets the theme
    /// </summary>
    /// <param name="mode">Light, Dark, Darker or System</param>
    internal void SetBaseTheme(ThemeType mode)
    {
        //Get the Windows accent color
        _titleBrush = (SolidColorBrush)SystemParameters.WindowGlassBrush;

        //Retrieve the app's existing theme
        PaletteHelper paletteHelper = new();
        ITheme theme = paletteHelper.GetTheme();

        if (mode == ThemeType.System)
        {
            mode = GetSystemTheme().Equals("light") ? ThemeType.Light : ThemeType.Dark;
        }

        switch (mode)
        {
            case ThemeType.Light:
                theme.SetBaseTheme(Theme.Light);
                theme.Paper = Colors.WhiteSmoke;
                BorderBackgroundBrush = _titleBrush;
                break;
            case ThemeType.Dark:
                theme.SetBaseTheme(Theme.Dark);
                BorderBackgroundBrush = _titleBrush;
                break;
            case ThemeType.Darker:
                // Set card and paper background colors a bit darker
                theme.SetBaseTheme(Theme.Dark);
                theme.Body = (Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFcccccc");
                theme.Paper = (Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF202020");
                theme.CardBackground = (Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF141414");
                BorderForegroundBrush = new SolidColorBrush(theme.Body);
                BorderBackgroundBrush = new SolidColorBrush(theme.CardBackground);
                break;
            default:
                theme.SetBaseTheme(Theme.Light);
                break;
        }

        //Change the app's current theme
        paletteHelper.SetTheme(theme);
    }
    #endregion Set light, dark or darker theme

    #region Set primary accent color
    /// <summary>
    /// Sets the MDIX primary accent color
    /// </summary>
    /// <param name="color">One of the 18 color values</param>
    internal static void SetPrimaryColor(AccentColor color)
    {
        PaletteHelper paletteHelper = new();
        ITheme theme = paletteHelper.GetTheme();
        PrimaryColor primary = color switch
        {
            AccentColor.Red => PrimaryColor.Red,
            AccentColor.Pink => PrimaryColor.Pink,
            AccentColor.Purple => PrimaryColor.Purple,
            AccentColor.DeepPurple => PrimaryColor.DeepPurple,
            AccentColor.Indigo => PrimaryColor.Indigo,
            AccentColor.Blue => PrimaryColor.Blue,
            AccentColor.LightBlue => PrimaryColor.LightBlue,
            AccentColor.Cyan => PrimaryColor.Cyan,
            AccentColor.Teal => PrimaryColor.Teal,
            AccentColor.Green => PrimaryColor.Green,
            AccentColor.LightGreen => PrimaryColor.LightGreen,
            AccentColor.Lime => PrimaryColor.Lime,
            AccentColor.Yellow => PrimaryColor.Yellow,
            AccentColor.Amber => PrimaryColor.Amber,
            AccentColor.Orange => PrimaryColor.Orange,
            AccentColor.DeepOrange => PrimaryColor.DeepOrange,
            AccentColor.Brown => PrimaryColor.Brown,
            AccentColor.Grey => PrimaryColor.Grey,
            AccentColor.BlueGray => PrimaryColor.BlueGrey,
            _ => PrimaryColor.Blue,
        };
        Color primaryColor = SwatchHelper.Lookup[(MaterialDesignColor)primary];
        theme.SetPrimaryColor(primaryColor);
        paletteHelper.SetTheme(theme);
    }
    #endregion Set primary accent color

    #region UI scale converter
    /// <summary>
    /// Sets the value for UI scaling
    /// </summary>
    /// <param name="size">One of 5 values</param>
    /// <returns>double used by LayoutTransform</returns>
    internal static double UIScale(MySize size)
    {
        switch (size)
        {
            case MySize.Smallest:
                return 0.8;
            case MySize.Smaller:
                return 0.9;
            case MySize.Small:
                return 0.95;
            case MySize.Default:
                return 1.0;
            case MySize.Large:
                return 1.05;
            case MySize.Larger:
                return 1.1;
            case MySize.Largest:
                return 1.2;
            default:
                return 1.0;
        }
    }
    #endregion UI scale converter

    #region Navigation
    /// <summary>
    /// Navigates to the requested dialog or window
    /// </summary>
    /// <param name="selectedIndex"></param>
    public void NavigateToPage(NavPage selectedIndex)
    {
        switch (selectedIndex)
        {
            case NavPage.Logs:
                _ = MainFrame.Navigate(new LogPage());
                break;

            case NavPage.Settings:
                _ = MainFrame.Navigate(new SettingsPage());
                break;

            case NavPage.About:
                DialogHelpers.ShowAboutDialog();
                LbxNavigation.SelectedItem = null;
                break;

            case NavPage.Exit:
                App.ExplicitClose = true;
                Close();
                break;

            case NavPage.Start:
                Watch.StartWatcher();
                LbxNavigation.SelectedItem = null;
                break;

            case NavPage.Stop:
                Watch.StopWatcher();
                LbxNavigation.SelectedItem = null;
                break;
        }
    }

    private void LbxNavigation_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        NavigateToPage((NavPage)LbxNavigation.SelectedIndex);
    }
    #endregion Navigation

    #region Window Events
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _log.ConditionalDebug("Window Loaded");
    }

    private void Window_StateChanged(object sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized && UserSettings.Setting.MinimizeToTray)
        {
            Hide();
        }
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        // Stop the file system Watcher
        Watch.DisposeWatcher();

        //clean up notify icon (would otherwise stay after application ends)
        tbIcon.Dispose();

        // Stop the stopwatch and record elapsed time
        _stopwatch.Stop();
        _log.Info($"{AppInfo.AppName} is shutting down.  Elapsed time: {_stopwatch.Elapsed:h\\:mm\\:ss\\.ff}");

        // Shut down NLog
        LogManager.Shutdown();

        UserSettings.Setting.WindowLeft = Math.Floor(Left);
        UserSettings.Setting.WindowTop = Math.Floor(Top);
        UserSettings.Setting.WindowWidth = Math.Floor(ActualWidth);
        UserSettings.Setting.WindowHeight = Math.Floor(ActualHeight);

        // Save settings
        UserSettings.SaveSettings();
    }
    #endregion Window Events

    #region Startup Checks
    /// <summary>Check some settings before starting</summary>
    private void StartUpChecks()
    {
        if (CheckFoldersAndExt())
        {
            if (UserSettings.Setting.CheckOnStartup)
            {
                Watch.CheckOnDemand();
            }

            if (UserSettings.Setting.WatchOnStart)
            {
                Watch.StartWatcher();
            }
            else
            {
                UpdateStartStopMenu(false);
            }
        }

        if (UserSettings.Setting.Heartbeat)
        {
            Heartbeat.StartHeartbeat();
        }
    }
    #endregion Startup Checks

    #region Window Title
    /// <summary>
    /// Puts the version number in the title bar as well as Administrator if running elevated
    /// </summary>
    public void WindowTitleVersionAdmin()
    {
        // Set the windows title
        if (IsAdministrator())
        {
            Title = AppInfo.AppProduct + " - " + AppInfo.TitleVersion + " - (Administrator)";
        }
        else
        {
            Title = AppInfo.AppProduct + " - " + AppInfo.TitleVersion;
        }
    }
    #endregion Window Title

    #region Running as Administrator?
    /// <summary>
    /// Determines if running as administrator (elevated)
    /// </summary>
    /// <returns>True if running elevated</returns>
    public static bool IsAdministrator()
    {
        return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }
    #endregion Running as Administrator?

    #region Unhandled Exception Handler
    /// <summary>
    /// Handles any exceptions that weren't caught by a try-catch statement
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        _log.Error("Unhandled Exception");
        Exception e = (Exception)args.ExceptionObject;
        _log.Error(e.Message);
        if (e.InnerException != null)
        {
            _log.Error(e.InnerException.ToString());
        }
        _log.Error(e.StackTrace);

        _ = System.Windows.MessageBox.Show("An error has occurred. See the log file",
            "ERROR",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
    #endregion Unhandled Exception Handler

    #region Session ending
    /// <summary>
    /// Save settings and Log reason for shutdown.
    /// </summary>
    private void Current_SessionEnding(object sender, SessionEndingCancelEventArgs e)
    {
        App.ExplicitClose = true;
        UserSettings.SaveSettings();
        _log.Info($"{AppInfo.AppName} is stopping due to {e.ReasonSessionEnding}");
    }
    #endregion Session ending

    #region On Closing
    /// <summary>Overrides the Closing event.</summary>
    /// <param name="e">CancelEventArgs that contains the event data.</param>
    protected override void OnClosing(CancelEventArgs e)
    {
        if (App.ExplicitClose || !UserSettings.Setting.ConfirmExit)
        {
            e.Cancel = false;
            base.OnClosing(e);
        }
        else
        {
            MDCustMsgBox mbox = new("Do you want to exit Download Folder Watcher?",
                    "Confirm Closing",
                    ButtonType.YesNo,
                    true,
                    true,
                    this,
                    false);
            _ = mbox.ShowDialog();

            if (MDCustMsgBox.CustResult == CustResultType.No)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
                base.OnClosing(e);
            }
        }
    }
    #endregion On Closing

    #region Log watcher error
    /// <summary>
    /// Handles the Error event of the Watcher control.
    /// </summary>
    private void Watcher_Error(object sender, ErrorEventArgs e)
    {
        sbStatus.Content = "Error. See the log file.";
        _log.Error($"FileWatcher reports an error {e.GetException()}");
    }
    #endregion Log watcher error

    #region RoutedUICommand methods
    private void Command_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    private void StopCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = Watch.Watcher.EnableRaisingEvents;
    }

    private void StartCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = !Watch.Watcher.EnableRaisingEvents;
    }

    private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void QuitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Close();
    }

    private void ShowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        MainWindowHelpers.ShowMainWindow();
    }

    private void StopWatching_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Watch.StopWatcher();
    }

    private void StartWatching_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Watch.StartWatcher();
    }
    #endregion RoutedUICommand methods

    #region Add/Remove from registry
    /// <summary>
    /// Add a registry item to start DFWatch with Windows
    /// </summary>
    private void AddStartToRegistry()
    {
        if (IsLoaded && !RegRun.RegRunEntry("DFWatch"))
        {
            string result = RegRun.AddRegEntry("DFWatch", AppInfo.AppProcessPath);
            if (result == "OK")
            {
                _log.Info(@"DFWatch added to HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                MDCustMsgBox mbox = new("DFWatch will now start with Windows.",
                                    "DFWatch",
                                    ButtonType.Ok,
                                    true,
                                    true,
                                    this,
                                    false);
                _ = mbox.ShowDialog();
            }
            else
            {
                _log.Error($"DFWatch add to startup failed: {result}");
                MDCustMsgBox mbox = new("An error has occurred. See the log file.",
                                    "DFWatch Error",
                                    ButtonType.Ok,
                                    true,
                                    true,
                                    this,
                                    true);
                _ = mbox.ShowDialog();
            }
        }
    }

    /// <summary>
    /// Remove the registry item
    /// </summary>
    private void RemoveStartFromRegistry()
    {
        if (IsLoaded)
        {
            string result = RegRun.RemoveRegEntry("DFWatch");
            if (result == "OK")
            {
                _log.Info(@"DFWatch removed from HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run");

                MDCustMsgBox mbox = new("DFWatch was removed from Windows startup.",
                                    "DFWatch",
                                    ButtonType.Ok,
                                    true,
                                    true,
                                    this,
                                    false);
                _ = mbox.ShowDialog();
            }
            else
            {
                _log.Error($"DFWatch remove from startup failed: {result}");
                MDCustMsgBox mbox = new("An error has occurred. See the log file.",
                                    "DFWatch Error",
                                    ButtonType.Ok,
                                    true,
                                    true,
                                    this,
                                    true);
                _ = mbox.ShowDialog();
            }
        }
    }
    #endregion Add/Remove from registry

    #region Disappearing message in status bar
    /// <summary>
    /// Displays the message which then fades out after 5 seconds.
    /// </summary>
    /// <remarks>
    /// Inspired by https://stackoverflow.com/a/1601994/15237757.
    /// </remarks>
    /// <param name="msg">The message to be displayed</param>
    public void DisappearingMessage(string msg)
    {
        if (_msgTimer.IsEnabled)
        {
            _msgTimer.Stop();
            sbMessage.Visibility = Visibility.Collapsed;
        }
        sbMessage.Content = msg;
        sbMessage.Visibility = Visibility.Visible;

        _msgTimer.Interval = TimeSpan.FromSeconds(5);
        _msgTimer.Tick += MsgTimer_Tick;
        _msgTimer.Start();
    }

    private void MsgTimer_Tick(object sender, EventArgs e)
    {
        _msgTimer.Stop();
        sbMessage.Visibility = Visibility.Collapsed;
    }
    #endregion Disappearing message in status bar

    #region Status message
    /// <summary>
    /// Sets the status message on the left side of the status menu.
    /// </summary>
    /// <param name="message">The message.</param>
    public void SetStatusMsg(string message)
    {
        sbStatus.Content = $"Status:  {message}";
    }
    #endregion Status message

    #region Update the tray icon tooltip to show Running or Stopped
    /// <summary>
    /// Sets the text of the tray icon tooltip.
    /// </summary>
    /// <param name="message">The message.</param>
    public void SetIcon(string message)
    {
        tbIcon.ToolTipText = $"DFWatch {AppInfo.TitleVersion} - {message}";
    }
    #endregion Update the tray icon tooltip to show Running or Stopped

    #region Enable/Disable start and stop in navigation menu
    /// <summary>Updates Start and Stop in the navigation menu.</summary>
    /// <param name="value">if set to <c>true</c> [value].</param>
    public void UpdateStartStopMenu(bool value)
    {
        if (value)
        {
            lbiStart.IsEnabled = false;
            lbiStop.IsEnabled = true;
        }
        else
        {
            lbiStart.IsEnabled = true;
            lbiStop.IsEnabled = false;
        }
    }
    #endregion Enable/Disable start and stop in navigation menu

    #region NLog "Method" target - writes to the message queue
    /// <summary>
    /// Writes to the message queue
    /// </summary>
    /// <param name="level">The message level.</param>
    /// <param name="message">The message to be written.</param>
    public static void LogMethod(string level, string message)
    {
        if (message.Contains("Setting Change", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _ = Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            if (MsgQueue.MessageQueue is not null)
            {
                try
                {
                    string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
                    MsgQueue.MessageQueue.Enqueue($"{date}  {level}  {message}");
                    if (MsgQueue.MessageQueue.Count > 100)
                    {
                        _ = MsgQueue.MessageQueue.Dequeue();
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "Error while adding message to queue.");
                }
            }
        }));
    }
    #endregion NLog "Method" target - writes to the message queue

    #region Double-click status bar for optimal window size
    /// <summary>
    /// Handles the MouseDoubleClick event of the status bar
    /// </summary>
    private void Sbar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        SizeToContent = SizeToContent.Width;
        double width = ActualWidth;
        Thread.Sleep(50);
        SizeToContent = SizeToContent.Manual;
        Width = width + 1;
    }
    #endregion Double-click status bar for optimal window size

    #region Key down events
    /// <summary>
    /// Keyboard events
    /// </summary>
    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.L && Keyboard.Modifiers == ModifierKeys.Control)
        {
            NavigateToPage(NavPage.Logs);
        }
        if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
        {
            Watch.StartWatcher();
            DisappearingMessage("Watcher Started");
        }
        if (e.Key == Key.P && Keyboard.Modifiers == ModifierKeys.Control)
        {
            Watch.StopWatcher();
            DisappearingMessage("Watcher Stopped");
        }

        if (e.Key == Key.Add && Keyboard.Modifiers == ModifierKeys.Control)
        {
            EverythingLarger();
            DisappearingMessage($"Size changed to: {(MySize)UserSettings.Setting.UISize}");
        }
        if (e.Key == Key.Subtract && Keyboard.Modifiers == ModifierKeys.Control)
        {
            EverythingSmaller();
            DisappearingMessage($"Size changed to: {(MySize)UserSettings.Setting.UISize}");
        }
        if (e.Key == Key.F1)
        {
            if (!DialogHost.IsDialogOpen("MainDialogHost"))
            {
                DialogHelpers.ShowAboutDialog();
            }
            else
            {
                DialogHost.Close("MainDialogHost");
                DialogHelpers.ShowAboutDialog();
            }
        }

        if (e.Key == Key.OemComma && Keyboard.Modifiers == ModifierKeys.Control)
        {
            NavigateToPage(NavPage.Settings);
        }

        if (e.Key == Key.M && Keyboard.Modifiers == ModifierKeys.Control)
        {
            switch (UserSettings.Setting.DarkMode)
            {
                case (int)ThemeType.Light:
                    UserSettings.Setting.DarkMode = (int)ThemeType.Dark;
                    break;
                case (int)ThemeType.Dark:
                    UserSettings.Setting.DarkMode = (int)ThemeType.Darker;
                    break;
                case (int)ThemeType.Darker:
                    UserSettings.Setting.DarkMode = (int)ThemeType.System;
                    break;
                case (int)ThemeType.System:
                    UserSettings.Setting.DarkMode = (int)ThemeType.Light;
                    break;
            }
            DisappearingMessage($"Theme changed to: {(ThemeType)UserSettings.Setting.DarkMode}");
        }
    }
    #endregion Key down events

    #region Smaller/Larger
    /// <summary>
    /// Decreases the size of the UI
    /// </summary>
    public void EverythingSmaller()
    {
        int size = UserSettings.Setting.UISize;
        if (size > 0)
        {
            size--;
            UserSettings.Setting.UISize = size;
            double newSize = UIScale((MySize)size);
            MainGrid.LayoutTransform = new ScaleTransform(newSize, newSize);
        }
    }

    /// <summary>
    /// Increases the size of the UI
    /// </summary>
    public void EverythingLarger()
    {
        int size = UserSettings.Setting.UISize;
        if (size < (int)MySize.Largest)
        {
            size++;
            UserSettings.Setting.UISize = size;
            double newSize = UIScale((MySize)size);
            MainGrid.LayoutTransform = new ScaleTransform(newSize, newSize);
        }
    }
    #endregion Smaller/Larger

    #region Check folders and extension list
    /// <summary>
    /// Checks the extension list and the source and destination folders
    /// </summary>
    private static bool CheckFoldersAndExt()
    {
        if (UserSettings.Setting.ExtensionList is null
            || UserSettings.Setting.ExtensionList.Count == 0)
        {
            _log.Warn("No extensions found in extension list");
        }
        else
        {
            int count = 0;
            _log.Info("Extension List:");
            foreach (var extension in UserSettings.Setting.ExtensionList)
            {
                _log.Info($"  [{count}] {extension}");
                count++;
            }
        }
        if (string.IsNullOrEmpty(UserSettings.Setting.SourceFolder))
        {
            _log.Warn("Source folder not specified");
            return false;
        }
        if (string.IsNullOrEmpty(UserSettings.Setting.DesitinationFolder))
        {
            _log.Warn("Destination folder not specified");
            return false;
        }
        return true;
    }
    #endregion Check folders and extension list
}
