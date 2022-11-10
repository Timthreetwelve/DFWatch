// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;

public static class Watch
{
    public static readonly FileSystemWatcher watcher = new();
    private static readonly Logger log = LogManager.GetLogger("logTemp");

    public static void StartWatcher()
    {
        watcher.Path = UserSettings.Setting.SourceFolder;
        watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
        watcher.IncludeSubdirectories = false;
        watcher.Filter = "*.*";
        watcher.Created += File_Created;
        watcher.Renamed += File_Renamed;
        watcher.Deleted += File_Deleted;
        watcher.EnableRaisingEvents = true;
        (Application.Current.MainWindow as MainWindow)?.SetStausMsg("Running");
        (Application.Current.MainWindow as MainWindow)?.UpdateStartStopMenu(true);
        log.Info($"{AppInfo.AppName} has started watching {watcher.Path}.");
    }

    public static void StopWatcher()
    {
        if (watcher != null)
        {
            watcher.EnableRaisingEvents = false;
            (Application.Current.MainWindow as MainWindow)?.SetStausMsg("Stopped");
            (Application.Current.MainWindow as MainWindow)?.UpdateStartStopMenu(false);
            log.Info($"{AppInfo.AppName} has stopped watching {watcher.Path}.");
        }
    }

    public static void DisposeWatcher()
    {
        if (watcher != null)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();

            log.Info($"{AppInfo.AppName} has stopped watching {watcher.Path}.");
        }
    }

    private static void File_Renamed(object sender, RenamedEventArgs e)
    {
        log.Debug($"File renamed: {e.Name} ");

        string thisFileExt = (Path.GetExtension(e.Name) ?? string.Empty).ToLower();

        FileInfo fileInfo = new(e.FullPath);
        if (Files.CheckExtension(FileExt.ExtensionList, thisFileExt))
        {
            Files.MoveFile(fileInfo);
        }
        else
        {
            log.Debug($"\"{thisFileExt}\" in not in the list of file extensions. No action taken on file {e.Name}.");
        }
    }

    private static void File_Created(object sender, FileSystemEventArgs e)
    {
        log.Debug($"File created: {e.Name} ");

        string thisFileExt = (Path.GetExtension(e.Name) ?? string.Empty).ToLower();

        FileInfo fileInfo = new(e.FullPath);
        if (Files.CheckExtension(FileExt.ExtensionList, thisFileExt))
        {
            Files.MoveFile(fileInfo);
        }
        else
        {
            log.Debug($"\"{thisFileExt}\" in not in the list of file extensions. No action taken on file {e.Name}.");
        }
    }

    private static void File_Deleted(object sender, FileSystemEventArgs e)
    {
        log.Debug($"File deleted: {e.Name} ");
    }

    #region Check source folder on startup
    public static void CheckOnStart()
    {
        if (UserSettings.Setting.CheckOnStartup)
        {
            log.Info($"Checking for existing files in source folder ({UserSettings.Setting.SourceFolder}).");
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
                                log.Debug($"\"{thisFileExt}\" in not in the list of file extensions. No action taken on file {fi.Name}.");
                            }
                        }
                    }
                }
            }
        }
    }
    #endregion Check source folder on startup
}
