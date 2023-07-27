// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System.Drawing;

namespace DFWatch.Helpers;

public static class TrayIconHelpers
{
    /// <summary>
    /// Creates the tray icon.
    /// </summary>
    public static void CreateTrayIcon()
    {
        SystemTrayIcon.TrayIcon.Icon = Icon.ExtractAssociatedIcon(AppInfo.AppPath);
        SystemTrayIcon.TrayIcon.Visible = true;
        SystemTrayIcon.TrayIcon.Text = AppInfo.ToolTipVersion;
        SystemTrayIcon.TrayIcon.MouseClick += TrayIcon_MouseClick;
    }

    /// <summary>
    /// Handles the MouseClick event of the TrayIcon control.
    /// Left click will show main window, Right click will open context menu.
    /// </summary>
    private static void TrayIcon_MouseClick(object sender, Forms.MouseEventArgs e)
    {
        if (e.Button == Forms.MouseButtons.Right)
        {
            ContextMenu trayContextMenu = (ContextMenu)Application.Current.FindResource("TrayContextMenu");
            trayContextMenu.IsOpen = true;

            // Get context menu handle and bring it to the foreground
            if (PresentationSource.FromVisual(trayContextMenu) is HwndSource windowHandleSource)
            {
                _ = NativeMethods.SetForegroundWindow(windowHandleSource.Handle);
            }
        }
        else if (e.Button == Forms.MouseButtons.Left)
        {
            MainWindowHelpers.ShowMainWindow();
        }
    }
}
