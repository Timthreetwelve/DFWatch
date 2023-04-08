﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

// Comment out the following if MessageBox is not to be used
#define messagebox

namespace DFWatch.Helpers;

/// <summary>
///  Class for viewing text files. If the file extension is not associated
///  with an application, notepad.exe will be attempted.
/// </summary>
public static class TextFileViewer
{
    #region Text file viewer
    /// <summary>
    /// Opens specified text file
    /// </summary>
    /// <param name="txtfile">Full path for text file</param>
    ///
    public static void ViewTextFile(string txtfile)
    {
        if (File.Exists(txtfile))
        {
            try
            {
                using Process p = new();
                p.StartInfo.FileName = txtfile;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.ErrorDialog = false;
                _ = p.Start();
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == 1155)
                {
                    using Process p = new();
                    p.StartInfo.FileName = "notepad.exe";
                    p.StartInfo.Arguments = txtfile;
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.ErrorDialog = false;
                    _ = p.Start();
                    NLogHelpers.Log.Debug($"Opening {txtfile} in Notepad.exe");
                }
                else
                {
#if messagebox
                    _ = MessageBox.Show($"Error reading file {txtfile}\n{ex.Message}", "Watcher Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
#endif
                    NLogHelpers.Log.Error($"* Unable to open {txtfile}");
                    NLogHelpers.Log.Error($"* {ex.Message}");
                }
            }
            catch (Exception ex)
            {
#if messagebox
                _ = MessageBox.Show("Unable to start default application used to open" +
                                    $" {txtfile}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
                NLogHelpers.Log.Error($"* Unable to open {txtfile}");
                NLogHelpers.Log.Error($"* {ex.Message}");
            }
        }
        else
        {
            NLogHelpers.Log.Error($">>> File not found: {txtfile}");
        }
    }
    #endregion
}
