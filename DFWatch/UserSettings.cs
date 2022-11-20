﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;

public class UserSettings : SettingsManager<UserSettings>, INotifyPropertyChanged
{
    #region Methods
    public void SaveWindowPos()
    {
        Window mainWindow = Application.Current.MainWindow;
        WindowHeight = Math.Floor(mainWindow.Height);
        WindowLeft = Math.Floor(mainWindow.Left);
        WindowTop = Math.Floor(mainWindow.Top);
        WindowWidth = Math.Floor(mainWindow.Width);
    }

    public void SetWindowPos()
    {
        Window mainWindow = Application.Current.MainWindow;
        mainWindow.Height = WindowHeight;
        mainWindow.Left = WindowLeft;
        mainWindow.Top = WindowTop;
        mainWindow.Width = WindowWidth;
    }
    #endregion Methods

    #region Observable collection
    private ObservableCollection<string> extensionList = new() {".txt"};
    public ObservableCollection<string> ExtensionList
    {
        get => extensionList;
        set
        {
            extensionList = value;
            OnPropertyChanged();
        }
    }
    #endregion Observable collection

    #region Properties
    public bool CheckOnStartup
    {
        get => checkOnStartup;
        set
        {
            checkOnStartup = value;
            OnPropertyChanged();
        }
    }

    public bool ColoredMessages
    {
        get => coloredMessages;
        set
        {
            coloredMessages = value;
            OnPropertyChanged();
        }
    }

    public bool ConfirmExit
    {
        get => confirmExit;
        set
        {
            confirmExit = value;
            OnPropertyChanged();
        }
    }


    public int DarkMode
    {
        get => darkmode;
        set
        {
            darkmode = value;
            OnPropertyChanged();
        }
    }

    public string DesitinationFolder
    {
        get => destinationFolder;
        set
        {
            destinationFolder = value;
            OnPropertyChanged();
        }
    }

    public bool Heartbeat
    {
        get => heartbeat;
        set
        {
            heartbeat = value;
            OnPropertyChanged();
        }
    }

    public bool IncludeDebugInFile
    {
        get => includeDebugInFile;
        set
        {
            includeDebugInFile = value;
            OnPropertyChanged();
        }
    }

    public bool IncludeDebugInGui
    {
        get => includeDebugInGui;
        set
        {
            includeDebugInGui = value;
            OnPropertyChanged();
        }
    }

    public int InitialDelay
    {
        get
        {
            if (initialDelay < 100)
            {
                initialDelay = 100;
            }
            return initialDelay;
        }
        set
        {
            initialDelay = value;
            OnPropertyChanged();
        }
    }

    public bool KeepOnTop
    {
        get => keepOnTop;
        set
        {
            keepOnTop = value;
            OnPropertyChanged();
        }
    }

    public int LogFileSize
    {
        get => logFileSize;
        set
        {
            logFileSize = value;
            OnPropertyChanged();
        }
    }

    public int LogFileVersions
    {
        get => logFileVersions;
        set
        {
            logFileVersions = value;
            OnPropertyChanged();
        }
    }

    public bool MinimizeToTray
    {
        get => minimizeToTray;
        set
        {
            minimizeToTray = value;
            OnPropertyChanged();
        }
    }

    public bool MinimizeToTrayOnClose
    {
        get => minimizeToTrayOnClose;
        set
        {
            minimizeToTrayOnClose = value;
            OnPropertyChanged();
        }
    }

    public int NumRetries
    {
        get
        {
            if (numRetries < 0)
            {
                return 0;
            }
            return numRetries;
        }
        set
        {
            numRetries = value;
            OnPropertyChanged();
        }
    }

    public int PrimaryColor
    {
        get => primaryColor;
        set
        {
            primaryColor = value;
            OnPropertyChanged();
        }
    }

    public int RetryDelay
    {
        get
        {
            if (retryDelay < 250)
            {
                retryDelay = 250;
            }
            return retryDelay;
        }
        set
        {
            retryDelay = value;
            OnPropertyChanged();
        }
    }

    public string SourceFolder
    {
        get => sourceFolder;
        set
        {
            sourceFolder = value;
            OnPropertyChanged();
        }
    }

    public bool StartMinimized
    {
        get => startMinimized;
        set
        {
            startMinimized = value;
            OnPropertyChanged();
        }
    }

    public bool StartWithWindows
    {
        get => startWithWindows;
        set
        {
            startWithWindows = value;
            OnPropertyChanged();
        }
    }

    public int UISize
    {
        get => uiSize;
        set
        {
            uiSize = value;
            OnPropertyChanged();
        }
    }

    public bool WatchOnStart
    {
        get => watchOnStart;
        set
        {
            watchOnStart = value;
            OnPropertyChanged();
        }
    }

    public double WindowHeight
    {
        get
        {
            if (windowHeight < 100)
            {
                windowHeight = 100;
            }
            return windowHeight;
        }
        set => windowHeight = value;
    }

    public double WindowLeft
    {
        get
        {
            if (windowLeft < 0)
            {
                windowLeft = 100;
            }
            return windowLeft;
        }
        set => windowLeft = value;
    }

    public double WindowTop
    {
        get
        {
            if (windowTop < 0)
            {
                windowTop = 100;
            }
            return windowTop;
        }
        set => windowTop = value;
    }

    public double WindowWidth
    {
        get
        {
            if (windowWidth < 100)
            {
                windowWidth = 100;
            }
            return windowWidth;
        }
        set => windowWidth = value;
    }
    #endregion Properties

    #region Private backing fields
    private bool checkOnStartup = true;
    private bool coloredMessages = true;
    private bool confirmExit = true;
    private int darkmode = 0;
    private string destinationFolder = string.Empty;
    private bool heartbeat = true;
    private bool includeDebugInFile = true;
    private bool includeDebugInGui = true;
    private int initialDelay = 1000;
    private bool keepOnTop = false;
    private int logFileSize = 20;
    private int logFileVersions = 10;
    private bool minimizeToTray = false;
    private bool minimizeToTrayOnClose = false;
    private int numRetries = 5;
    private int primaryColor = (int)AccentColor.Blue;
    private int retryDelay = 1000;
    private string sourceFolder = string.Empty;
    private bool startMinimized = false;
    private bool startWithWindows = false;
    private int uiSize = (int)MySize.Default;
    private bool watchOnStart = false;
    private double windowHeight = 500;
    private double windowLeft = 200;
    private double windowTop = 200;
    private double windowWidth = 800;
    #endregion Private backing fields

    #region Handle property change event
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion Handle property change event
}
