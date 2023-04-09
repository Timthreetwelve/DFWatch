// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    #region Stopwatch
    private readonly Stopwatch _stopwatch = new();
    #endregion Stopwatch

    #region Private fields
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
    /// Initialize settings
    /// </summary>
    private void InitializeSettings()
    {
        _stopwatch.Start();

        ConfigHelpers.InitializeSettings();
    }

    /// <summary>
    /// Read and apply settings
    /// </summary>
    public void ReadSettings()
    {
        // Set NLog configuration
        NLogHelpers.NLogConfig();

        // Unhandled exception handler
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        // Put version number in window title
        Title = MainWindowHelpers.WindowTitleVersionAdmin();

        // Log the version, build date and commit id
        NLogHelpers.Log.Info($"{AppInfo.AppName} ({AppInfo.AppProduct}) {AppInfo.AppVersion} is starting up");
        NLogHelpers.Log.Info($"{AppInfo.AppName} {AppInfo.AppCopyright}");
        NLogHelpers.Log.Debug($"{AppInfo.AppName} Build date: {BuildInfo.BuildDateUtc.ToUniversalTime():f} (UTC)");
        NLogHelpers.Log.Debug($"{AppInfo.AppName} Commit ID: {BuildInfo.CommitIDString} ");

        // Log the .NET version, app framework and OS platform
        string version = Environment.Version.ToString();
        NLogHelpers.Log.Debug($".NET version: {AppInfo.RuntimeVersion.Replace(".NET", "")}  ({version})");
        NLogHelpers.Log.Debug(AppInfo.Framework);
        NLogHelpers.Log.Debug(AppInfo.OsPlatform);

        // Window position
        MainWindowHelpers.SetWindowPosition();

        // Light or dark
        MainWindowUIHelpers.SetBaseTheme(UserSettings.Setting.UITheme);

        // Primary accent color
        MainWindowUIHelpers.SetPrimaryColor(UserSettings.Setting.PrimaryColor);

        // UI size
        double size = MainWindowUIHelpers.UIScale(UserSettings.Setting.UISize);
        MainGrid.LayoutTransform = new ScaleTransform(size, size);

        // Settings change event
        UserSettings.Setting.PropertyChanged += SettingChange.UserSettingChanged;

        // Watch for errors
        Watch.Watcher.Error += Watcher_Error;

        // Session ending
        Application.Current.SessionEnding += Current_SessionEnding;

        // Window state changed
        StateChanged += MainWindow_StateChanged;

        // Tray Icon
        tbIcon.ForceCreate(enablesEfficiencyMode: true);
        tbIcon.Visibility = Visibility.Visible;


        if (UserSettings.Setting.StartMinimized)
        {
            WindowState = WindowState.Minimized;
            WindowExtensions.Hide(window: this, enableEfficiencyMode: true);
        }

        // select the 1st item in the navigation listbox
        NavigationListBox.SelectedIndex = 0;
    }
    #endregion Settings

    #region Window Events
    private void MainWindow_StateChanged(object sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
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
        NLogHelpers.Log.Info($"{AppInfo.AppName} is shutting down.  Elapsed time: {_stopwatch.Elapsed:h\\:mm\\:ss\\.ff}");

        // Shut down NLog
        LogManager.Shutdown();

        // Save settings
        MainWindowHelpers.SaveWindowPosition();
        ConfigHelpers.SaveSettings();
    }
    #endregion Window Events

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

    #region Session ending
    /// <summary>
    /// Save settings and Log reason for shutdown.
    /// </summary>
    private void Current_SessionEnding(object sender, SessionEndingCancelEventArgs e)
    {
        App.ExplicitClose = true;
        ConfigHelpers.SaveSettings();
        NLogHelpers.Log.Info($"{AppInfo.AppName} is stopping due to {e.ReasonSessionEnding}");
    }
    #endregion Session ending

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

    #region Startup Checks
    /// <summary>Check some settings before starting</summary>
    private static void StartUpChecks()
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
        }

        if (UserSettings.Setting.Heartbeat)
        {
            Heartbeat.StartHeartbeat();
        }
    }
    #endregion Startup Checks

    #region Check folders and extension list
    /// <summary>
    /// Checks the extension list and the source and destination folders
    /// </summary>
    private static bool CheckFoldersAndExt()
    {
        if (UserSettings.Setting.ExtensionList is null
            || UserSettings.Setting.ExtensionList.Count == 0)
        {
            NLogHelpers.Log.Warn("No extensions found in extension list");
        }
        else
        {
            int count = 0;
            NLogHelpers.Log.Info("Extension List:");
            foreach (var extension in UserSettings.Setting.ExtensionList)
            {
                NLogHelpers.Log.Info($"  [{count}] {extension}");
                count++;
            }
        }
        if (string.IsNullOrEmpty(UserSettings.Setting.SourceFolder))
        {
            NLogHelpers.Log.Warn("Source folder not specified");
            return false;
        }
        if (string.IsNullOrEmpty(UserSettings.Setting.DestinationFolder))
        {
            NLogHelpers.Log.Warn("Destination folder not specified");
            return false;
        }
        return true;
    }
    #endregion Check folders and extension list

    #region Log watcher error
    /// <summary>
    /// Handles the Error event of the Watcher control.
    /// </summary>
    private void Watcher_Error(object sender, ErrorEventArgs e)
    {
        sbStatus.Content = "Error. See the log file.";
        NLogHelpers.Log.Error($"FileWatcher reports an error {e.GetException()}");
    }
    #endregion Log watcher error

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
                    NLogHelpers.Log.Error(ex, "Error while adding message to queue.");
                }
            }
        }));
    }
    #endregion NLog "Method" target - writes to the message queue

    #region Smaller/Larger
    /// <summary>
    /// Decreases the size of the UI
    /// </summary>
    public void EverythingSmaller()
    {
        MySize size = UserSettings.Setting.UISize;
        if (size > 0)
        {
            size--;
            UserSettings.Setting.UISize = size;
            double newSize = MainWindowUIHelpers.UIScale(size);
            MainGrid.LayoutTransform = new ScaleTransform(newSize, newSize);
        }
    }

    /// <summary>
    /// Increases the size of the UI
    /// </summary>
    public void EverythingLarger()
    {
        MySize size = UserSettings.Setting.UISize;
        if (size < MySize.Largest)
        {
            size++;
            UserSettings.Setting.UISize = size;
            double newSize = MainWindowUIHelpers.UIScale((MySize)size);
            MainGrid.LayoutTransform = new ScaleTransform(newSize, newSize);
        }
    }
    #endregion Smaller/Larger

    #region RoutedUICommand methods
    private void StartCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = !Watch.Watcher.EnableRaisingEvents;
    }

    private void StartWatching_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Watch.StartWatcher();
    }

    private void StopCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = Watch.Watcher.EnableRaisingEvents;
    }

    private void StopWatching_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Watch.StopWatcher();
    }
    #endregion RoutedUICommand methods

    #region Unhandled Exception Handler
    /// <summary>
    /// Handles any exceptions that weren't caught by a try-catch statement
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        NLogHelpers.Log.Error("Unhandled Exception");
        Exception e = (Exception)args.ExceptionObject;
        NLogHelpers.Log.Error(e.Message);
        if (e.InnerException != null)
        {
            NLogHelpers.Log.Error(e.InnerException.ToString());
        }
        NLogHelpers.Log.Error(e.StackTrace);

        _ = MessageBox.Show("An error has occurred. See the log file",
            "ERROR",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
    #endregion Unhandled Exception Handler
}
