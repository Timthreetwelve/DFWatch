// Copyright (c) Tim Kennedy. CleanAll Rights Reserved. Licensed under the MIT License.

namespace DFWatch.Configuration;

[INotifyPropertyChanged]
public partial class UserSettings : ConfigManager<UserSettings>
{
    #region Observable Collection
    private ObservableCollection<string> _extensionList = new() { ".txt" };
    public ObservableCollection<string> ExtensionList
    {
        get => _extensionList;
        set => SetProperty(ref _extensionList, value);
    }
    #endregion Observable Collection

    #region Properties
    [ObservableProperty]
    private bool _checkOnStartup = true;

    [ObservableProperty]
    private bool _coloredMessages = true;

    [ObservableProperty]
    private bool _confirmExit = true;

    [ObservableProperty]
    private string _destinationFolder = string.Empty;

    [ObservableProperty]
    private bool _heartbeat = true;

    [ObservableProperty]
    private bool _includeDebugInFile = true;

    [ObservableProperty]
    private bool _includeDebugInGui = true;

    [ObservableProperty]
    private int _initialDelay = 1000;

    [ObservableProperty]
    private bool _keepOnTop;

    [ObservableProperty]
    private int _logFileSize = 20;

    [ObservableProperty]
    private int _logFileVersions = 10;

    [ObservableProperty]
    private bool _minimizeToTrayOnClose;

    [ObservableProperty]
    private int _numRetries = 5;

    [ObservableProperty]
    private AccentColor _primaryColor = AccentColor.Blue;

    [ObservableProperty]
    private bool _renameIfDuplicate = true;

    [ObservableProperty]
    private int _retryDelay = 1000;

    [ObservableProperty]
    private string _sourceFolder = string.Empty;

    [ObservableProperty]
    private bool _startMinimized;

    [ObservableProperty]
    private bool _startWithWindows;

    [ObservableProperty]
    private MySize _uISize = MySize.Default;

    [ObservableProperty]
    private ThemeType _uITheme = ThemeType.System;

    [ObservableProperty]
    private bool _watchOnStart;

    [ObservableProperty]
    private double _windowHeight = 500;

    [ObservableProperty]
    private double _windowLeft = 100;

    [ObservableProperty]
    private double _windowTop = 100;

    [ObservableProperty]
    private double _windowWidth = 850;
    #endregion Properties
}
