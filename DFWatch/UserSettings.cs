// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

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
    private ObservableCollection<string> _extensionList = new() { ".txt" };
    public ObservableCollection<string> ExtensionList
    {
        get => _extensionList;
        set
        {
            _extensionList = value;
            OnPropertyChanged();
        }
    }
    #endregion Observable collection

    #region Properties
    public bool CheckOnStartup
    {
        get => _checkOnStartup;
        set
        {
            _checkOnStartup = value;
            OnPropertyChanged();
        }
    }

    public bool ColoredMessages
    {
        get => _coloredMessages;
        set
        {
            _coloredMessages = value;
            OnPropertyChanged();
        }
    }

    public bool ConfirmExit
    {
        get => _confirmExit;
        set
        {
            _confirmExit = value;
            OnPropertyChanged();
        }
    }

    public int DarkMode
    {
        get => _darkmode;
        set
        {
            _darkmode = value;
            OnPropertyChanged();
        }
    }

    public string DesitinationFolder
    {
        get => _destinationFolder;
        set
        {
            _destinationFolder = value;
            OnPropertyChanged();
        }
    }

    public bool Heartbeat
    {
        get => _heartbeat;
        set
        {
            _heartbeat = value;
            OnPropertyChanged();
        }
    }

    public bool IncludeDebugInFile
    {
        get => _includeDebugInFile;
        set
        {
            _includeDebugInFile = value;
            OnPropertyChanged();
        }
    }

    public bool IncludeDebugInGui
    {
        get => _includeDebugInGui;
        set
        {
            _includeDebugInGui = value;
            OnPropertyChanged();
        }
    }

    public int InitialDelay
    {
        get
        {
            if (_initialDelay < 100)
            {
                _initialDelay = 100;
            }
            return _initialDelay;
        }
        set
        {
            _initialDelay = value;
            OnPropertyChanged();
        }
    }

    public bool KeepOnTop
    {
        get => _keepOnTop;
        set
        {
            _keepOnTop = value;
            OnPropertyChanged();
        }
    }

    public int LogFileSize
    {
        get => _logFileSize;
        set
        {
            _logFileSize = value;
            OnPropertyChanged();
        }
    }

    public int LogFileVersions
    {
        get => _logFileVersions;
        set
        {
            _logFileVersions = value;
            OnPropertyChanged();
        }
    }

    public bool MinimizeToTray
    {
        get => _minimizeToTray;
        set
        {
            _minimizeToTray = value;
            OnPropertyChanged();
        }
    }

    public bool MinimizeToTrayOnClose
    {
        get => _minimizeToTrayOnClose;
        set
        {
            _minimizeToTrayOnClose = value;
            OnPropertyChanged();
        }
    }

    public int NumRetries
    {
        get
        {
            if (_numRetries < 0)
            {
                return 0;
            }
            return _numRetries;
        }
        set
        {
            _numRetries = value;
            OnPropertyChanged();
        }
    }

    public int PrimaryColor
    {
        get => _primaryColor;
        set
        {
            _primaryColor = value;
            OnPropertyChanged();
        }
    }

    public bool RenameIfDuplicate
    {
        get => _renameIfDuplicate;
        set
        {
            _renameIfDuplicate = value;
            OnPropertyChanged();
        }
    }

    public int RetryDelay
    {
        get
        {
            if (_retryDelay < 250)
            {
                _retryDelay = 250;
            }
            return _retryDelay;
        }
        set
        {
            _retryDelay = value;
            OnPropertyChanged();
        }
    }

    public string SourceFolder
    {
        get => _sourceFolder;
        set
        {
            _sourceFolder = value;
            OnPropertyChanged();
        }
    }

    public bool StartMinimized
    {
        get => _startMinimized;
        set
        {
            _startMinimized = value;
            OnPropertyChanged();
        }
    }

    public bool StartWithWindows
    {
        get => _startWithWindows;
        set
        {
            _startWithWindows = value;
            OnPropertyChanged();
        }
    }

    public int UISize
    {
        get => _uiSize;
        set
        {
            _uiSize = value;
            OnPropertyChanged();
        }
    }

    public bool WatchOnStart
    {
        get => _watchOnStart;
        set
        {
            _watchOnStart = value;
            OnPropertyChanged();
        }
    }

    public double WindowHeight
    {
        get
        {
            if (_windowHeight < 100)
            {
                _windowHeight = 100;
            }
            return _windowHeight;
        }
        set => _windowHeight = value;
    }

    public double WindowLeft
    {
        get
        {
            if (_windowLeft < 0)
            {
                _windowLeft = 100;
            }
            return _windowLeft;
        }
        set => _windowLeft = value;
    }

    public double WindowTop
    {
        get
        {
            if (_windowTop < 0)
            {
                _windowTop = 100;
            }
            return _windowTop;
        }
        set => _windowTop = value;
    }

    public double WindowWidth
    {
        get
        {
            if (_windowWidth < 100)
            {
                _windowWidth = 100;
            }
            return _windowWidth;
        }
        set => _windowWidth = value;
    }
    #endregion Properties

    #region Private backing fields
    private bool _checkOnStartup = true;
    private bool _coloredMessages = true;
    private bool _confirmExit = true;
    private int _darkmode = 0;
    private string _destinationFolder = string.Empty;
    private bool _heartbeat = true;
    private bool _includeDebugInFile = true;
    private bool _includeDebugInGui = true;
    private int _initialDelay = 1000;
    private bool _keepOnTop = false;
    private int _logFileSize = 20;
    private int _logFileVersions = 10;
    private bool _minimizeToTray = false;
    private bool _minimizeToTrayOnClose = false;
    private int _numRetries = 5;
    private int _primaryColor = (int)AccentColor.Blue;
    private bool _renameIfDuplicate = true;
    private int _retryDelay = 1000;
    private string _sourceFolder = string.Empty;
    private bool _startMinimized = false;
    private bool _startWithWindows = false;
    private int _uiSize = (int)MySize.Default;
    private bool _watchOnStart = false;
    private double _windowHeight = 500;
    private double _windowLeft = 200;
    private double _windowTop = 200;
    private double _windowWidth = 800;
    #endregion Private backing fields

    #region Handle property change event
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion Handle property change event
}
