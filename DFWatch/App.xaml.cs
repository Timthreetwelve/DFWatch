// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    #region Explicit close property
    internal static bool ExplicitClose { get; set; }
    #endregion Explicit close property

    #region Single instance
    /// <summary>Override the Startup Event</summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        SingleInstance.Create(AppInfo.AppName);

        base.OnStartup(e);
    }
    #endregion Single instance
}
