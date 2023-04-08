// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch.Models;

/// <summary>Class for controlling the file system Watcher.</summary>
public static class Watch
{
    #region The Watcher
    /// <summary>Gets the file system watcher.</summary>
    /// <value>The watcher.</value>
    public static FileSystemWatcher Watcher { get; } = new();
    #endregion The Watcher

    #region Start method
    /// <summary>Starts the file system Watcher.</summary>
    public static void StartWatcher()
    {
        Watcher.Path = UserSettings.Setting.SourceFolder;
        Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
        Watcher.IncludeSubdirectories = false;
        Watcher.Filter = "*.*";
        Watcher.Created += File_Created;
        Watcher.Renamed += File_Renamed;
        Watcher.Deleted += File_Deleted;
        Watcher.EnableRaisingEvents = true;
        (Application.Current.MainWindow as MainWindow)?.SetStatusMsg("Running");
        NLogHelpers.Log.Info($"{AppInfo.AppName} has started watching {Watcher.Path}.");
    }
    #endregion Start method

    #region Stop method
    /// <summary>Stops the file system Watcher.</summary>
    public static void StopWatcher()
    {
        if (Watcher != null)
        {
            Watcher.EnableRaisingEvents = false;
            (Application.Current.MainWindow as MainWindow)?.SetStatusMsg("Stopped");
            NLogHelpers.Log.Info($"{AppInfo.AppName} has stopped watching {Watcher.Path}.");
        }
    }
    #endregion Stop method

    #region Dispose method
    /// <summary>Disposes the file system Watcher.</summary>
    public static void DisposeWatcher()
    {
        if (Watcher != null)
        {
            Watcher.EnableRaisingEvents = false;
            Watcher.Dispose();

            NLogHelpers.Log.Info($"{AppInfo.AppName} has stopped watching {Watcher.Path}.");
        }
    }
    #endregion Dispose method

    #region Check source folder on startup
    /// <summary>Check for existing files on demand</summary>
    public static void CheckOnDemand()
    {
        NLogHelpers.Log.Info($"Checking for existing files in source folder ({UserSettings.Setting.SourceFolder}).");
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
                            NLogHelpers.Log.Debug($"\"{thisFileExt}\" in not in the list of file extensions. No action taken on file {fi.Name}.");
                        }
                    }
                }
            }
        }
    }
    #endregion Check source folder on startup

    #region File renamed event
    /// <summary>Handles the Renamed event</summary>
    private static void File_Renamed(object sender, RenamedEventArgs e)
    {
        NLogHelpers.Log.Debug($"File renamed: {e.Name} ");

        string thisFileExt = (Path.GetExtension(e.Name) ?? string.Empty).ToLower();

        FileInfo fileInfo = new(e.FullPath);
        if (Files.CheckExtension(FileExt.ExtensionList, thisFileExt))
        {
            Files.MoveFile(fileInfo);
        }
        else
        {
            NLogHelpers.Log.Debug($"\"{thisFileExt}\" in not in the list of file extensions. No action taken on file {e.Name}.");
        }
    }
    #endregion File renamed event

    #region File created event
    /// <summary>Handles the Created event.</summary>
    private static void File_Created(object sender, FileSystemEventArgs e)
    {
        NLogHelpers.Log.Debug($"File created: {e.Name} ");

        string thisFileExt = (Path.GetExtension(e.Name) ?? string.Empty).ToLower();

        FileInfo fileInfo = new(e.FullPath);
        if (Files.CheckExtension(FileExt.ExtensionList, thisFileExt))
        {
            Files.MoveFile(fileInfo);
        }
        else
        {
            NLogHelpers.Log.Debug($"\"{thisFileExt}\" in not in the list of file extensions. No action taken on file {e.Name}.");
        }
    }
    #endregion File created event

    #region File deleted event
    /// <summary>File Deleted event.</summary>
    /// <remarks>Just logging the deletion of a file for now.</remarks>
    private static void File_Deleted(object sender, FileSystemEventArgs e)
    {
        NLogHelpers.Log.Debug($"File deleted: {e.Name} ");
    }
    #endregion File deleted event
}
