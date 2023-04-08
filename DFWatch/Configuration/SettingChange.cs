// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch.Configuration
{
    public static class SettingChange
    {
        private static readonly MainWindow _mainWindow = Application.Current.MainWindow as MainWindow;

        #region Setting change
        /// <summary>
        /// Handle changes in UserSettings
        /// </summary>
        public static void UserSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            object newValue = MainWindowHelpers.GetPropertyValue(sender, e);
            NLogHelpers.Log.Debug($"Setting change: {e.PropertyName} New Value: {newValue}");
            switch (e.PropertyName)
            {
                case nameof(UserSettings.Setting.KeepOnTop):
                    _mainWindow.Topmost = (bool)newValue;
                    break;

                case nameof(UserSettings.Setting.IncludeDebugInFile):
                    NLogHelpers.SetLogToFileLevel((bool)newValue);
                    break;

                case nameof(UserSettings.Setting.IncludeDebugInGui):
                    NLogHelpers.SetLogToMethodLevel((bool)newValue);
                    break;

                case nameof(UserSettings.Setting.UITheme):
                    MainWindowUIHelpers.SetBaseTheme((ThemeType)newValue);
                    break;

                //case nameof(UserSettings.Setting.MinimizeToTray):
                //    _mainWindow.tbIcon.Visibility = (bool)newValue ? Visibility.Visible : Visibility.Collapsed;
                //    break;

                case nameof(UserSettings.Setting.PrimaryColor):
                    MainWindowUIHelpers.SetPrimaryColor((AccentColor)newValue);
                    break;

                case nameof(UserSettings.Setting.UISize):
                    int size = (int)newValue;
                    double newSize = MainWindowUIHelpers.UIScale((MySize)size);
                    _mainWindow.MainGrid.LayoutTransform = new ScaleTransform(newSize, newSize);
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
    }
}
