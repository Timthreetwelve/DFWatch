// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System.Threading;

namespace DFWatch;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MaterialWindow
{
    #region Stopwatch
    private readonly Stopwatch stopwatch = new();
    #endregion Stopwatch

    #region NLog Instance
    private static readonly Logger log = LogManager.GetLogger("logTemp");
    #endregion NLog Instance

    #region Private fields
    private static SolidColorBrush TitleBrush;
    private readonly DispatcherTimer msgTimer = new();
    public static StatusBarItem status;
    #endregion Private fields

    public MainWindow()
    {
        InitializeSettings();

        InitializeComponent();

        ReadSettings();

        if (CheckFoldersAndExt())
        {
            CheckOnStart();

            if (UserSettings.Setting.WatchOnStart)
            {
                Watch.StartWatcher();
            }
        }

        Heartbeat.StartHeartbeat();
    }

    #region Check folders and extension list
    /// <summary>
    /// Checks the settings. - This might need to move to a separate file.
    /// </summary>
    private static bool CheckFoldersAndExt()
    {
        if (UserSettings.Setting.ExtensionList is null
            || UserSettings.Setting.ExtensionList.Count == 0)
        {
            log.Warn("No extensions found in extension list");
        }
        else
        {
            int count = 0;
            log.Debug("Extension List:");
            foreach (var extension in UserSettings.Setting.ExtensionList)
            {
                log.Info($"  [{count}] {extension}");
                count++;
            }
        }
        if (string.IsNullOrEmpty(UserSettings.Setting.SourceFolder))
        {
            log.Warn("Source folder not specified");
            return false;
        }
        if (string.IsNullOrEmpty(UserSettings.Setting.DesitinationFolder))
        {
            log.Warn("Destination folder not specified");
            return false;
        }
        return true;
    }
    #endregion Check folders and extension list

    #region Settings
    /// <summary>
    /// Read and apply settings
    /// </summary>
    private void InitializeSettings()
    {
        stopwatch.Start();

        UserSettings.Init(UserSettings.AppFolder, UserSettings.DefaultFilename, true);
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

        // Put version number in window title
        WindowTitleVersionAdmin();

        // Log the version, build date and commit id
        log.Info($"{AppInfo.AppName} ({AppInfo.AppProduct}) {AppInfo.AppVersion} is starting up");
        log.Info($"{AppInfo.AppName} {AppInfo.AppCopyright}");
        log.Debug($"{AppInfo.AppName} Build date: {BuildInfo.BuildDateUtc.ToUniversalTime():g} (UTC)");
        log.Debug($"{AppInfo.AppName} Commit ID: {BuildInfo.CommitIDString} ");

        // Log the .NET version, app framework and OS platform
        string version = Environment.Version.ToString();
        log.Debug($".NET version: {AppInfo.RuntimeVersion.Replace(".NET", "")}  ({version})");
        log.Debug(AppInfo.Framework);
        log.Debug(AppInfo.OsPlatform);

        // Window position
        UserSettings.Setting.SetWindowPos();
        Topmost = UserSettings.Setting.KeepOnTop;

        // Settings change event
        UserSettings.Setting.PropertyChanged += UserSettingChanged;

        //Start with main page
        NavigateToPage(NavPage.Main);

        // Watch for errors
        Watch.watcher.Error += Watcher_Error;

        // Initial status message
        SetStausMsg("Stopped");
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
        log.Debug($"Setting change: {e.PropertyName} New Value: {newValue}");
        switch (e.PropertyName)
        {
            case nameof(UserSettings.Setting.KeepOnTop):
                Topmost = (bool)newValue;
                break;

            case nameof(UserSettings.Setting.IncludeDebug):
                NLHelpers.SetLogLevel((bool)newValue);
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

    #region Set light or dark theme
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
        //Get the accent color
        TitleBrush = (SolidColorBrush)SystemParameters.WindowGlassBrush;

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
                BorderBackgroundBrush = TitleBrush;
                break;
            case ThemeType.Dark:
                theme.SetBaseTheme(Theme.Dark);
                BorderBackgroundBrush = TitleBrush;
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
    #endregion Set light or dark theme

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
            case NavPage.Main:
                _ = MainFrame.Navigate(new MainPage());
                break;

            case NavPage.Logs:
                _ = MainFrame.Navigate(new LogsPage());
                break;

            case NavPage.Settings:
                _ = MainFrame.Navigate(new SettingsPage());
                break;

            case NavPage.About:
                DialogHelpers.ShowAboutDialog();
                LbxNavigation.SelectedItem = null;
                break;

            case NavPage.Exit:
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
    private void Window_Activated(object sender, EventArgs e)
    {
        // window activated stuff here
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        status = sbNLog;
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        // Stop the file system watcher
        Watch.DisposeWatcher();

        // Stop the stopwatch and record elapsed time
        stopwatch.Stop();
        log.Info($"{AppInfo.AppName} is shutting down.  Elapsed time: {stopwatch.Elapsed:h\\:mm\\:ss\\.ff}");

        // Shut down NLog
        LogManager.Shutdown();

        // Save settings
        UserSettings.Setting.SaveWindowPos();
        UserSettings.SaveSettings();
    }
    #endregion Window Events

    #region Window Title
    /// <summary>
    /// Puts the version number in the title bar as well as Administrator if running elevated
    /// </summary>
    public void WindowTitleVersionAdmin()
    {
        // Set the windows title
        if (IsAdministrator())
        {
            Title = AppInfo.AppName + " - " + AppInfo.TitleVersion + " - (Administrator)";
        }
        else
        {
            Title = AppInfo.AppName + " - " + AppInfo.TitleVersion;
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
        log.Error("Unhandled Exception");
        Exception e = (Exception)args.ExceptionObject;
        log.Error(e.Message);
        if (e.InnerException != null)
        {
            log.Error(e.InnerException.ToString());
        }
        log.Error(e.StackTrace);

        _ = System.Windows.MessageBox.Show("An error has occurred. See the log file",
            "ERROR",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
    #endregion Unhandled Exception Handler

    #region Log watcher error
    private void Watcher_Error(object sender, ErrorEventArgs e)
    {
        sbStatus.Content = "Error";
        log.Error($"FileWatcher reports an error {e.GetException()}");
    }
    #endregion Log watcher error

    #region Show Main window
    /// <summary>
    /// Show the main window and set it's state to normal
    /// </summary>
    private static void ShowMainWindow()
    {
        Application.Current.MainWindow.Show();
        Application.Current.MainWindow.Visibility = Visibility.Visible;
        Application.Current.MainWindow.WindowState = WindowState.Normal;
        _ = Application.Current.MainWindow.Activate();
    }

    private void TbIconShowMainWindow_Click(object sender, RoutedEventArgs e)
    {
        ShowMainWindow();
    }
    #endregion Show Main window

    #region RoutedUICommand methods
    private void Command_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    private void StopCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = Watch.watcher.EnableRaisingEvents;
    }

    private void StartCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = !Watch.watcher.EnableRaisingEvents;
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
        ShowMainWindow();
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
    /// Add a registry item to start My Launcher with Windows
    /// </summary>
    private void AddStartToRegistry()
    {
        if (IsLoaded && !RegRun.RegRunEntry("DFWatch"))
        {
            string result = RegRun.AddRegEntry("DFWatch", AppInfo.AppPath);
            if (result == "OK")
            {
                log.Info(@"DFWatch added to HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
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
                log.Error($"DFWatch add to startup failed: {result}");
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
                log.Info(@"DFWatch removed from HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run");

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
                log.Error($"DFWatch remove from startup failed: {result}");
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
    public void DisappearingMessage(string msg)
    {
        sbMessage.Content = msg;
        msgTimer.Interval = TimeSpan.FromSeconds(3);
        msgTimer.Tick += MsgTimer_Tick;
        msgTimer.Start();
    }

    private void MsgTimer_Tick(object sender, EventArgs e)
    {
        sbMessage.Content = string.Empty;
        msgTimer.Stop();
    }
    #endregion Disappearing message in status bar

    #region Status message
    public void SetStausMsg(string message)
    {
        sbStatus.Content = $"Status:  {message}";
    }
    #endregion Status message

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
                    Debug.WriteLine(ex);
                }
            }
        }));
    }

    #region Check source folder on startup
    private static void CheckOnStart()
    {
        if (UserSettings.Setting.CheckOnStartup)
        {
            log.Info("Checking for existing files in source folder.");
            string[] files = Directory.GetFiles(UserSettings.Setting.SourceFolder);
            FileExt.ExtensionList = UserSettings.Setting.ExtensionList;
            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    string thisFileExt = (Path.GetExtension(file) ?? string.Empty).ToLower();
                    if (thisFileExt != null)
                    {
                        FileInfo fi = new(file);
                        if ((fi.Attributes & FileAttributes.Hidden) == 0 || (fi.Attributes & FileAttributes.System) == 0)
                        {
                            if (Files.CheckExtension(FileExt.ExtensionList, thisFileExt))
                            {
                                Files.MoveFile(fi);
                            }
                            else
                            {
                                log.Debug($"{thisFileExt} in not in the list of extensions. No action taken on file {file}.");
                            }
                        }
                    }
                }
            }
        }
    }
    #endregion Check source folder on startup

    private void Sbar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        SizeToContent = SizeToContent.WidthAndHeight;
        double width = ActualWidth;
        Thread.Sleep(50);
        SizeToContent = SizeToContent.Manual;
        Width = width + 1;
    }
}
