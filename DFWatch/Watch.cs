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
        log.Info($"{AppInfo.AppName} has started watching {watcher.Path}.");
    }

    public static void StopWatcher()
    {
        if (watcher != null)
        {
            watcher.EnableRaisingEvents = false;
            (Application.Current.MainWindow as MainWindow)?.SetStausMsg("Stopped");
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
            log.Debug($"{thisFileExt} in not in the list of extensions. No action taken on file {e.Name}.");
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
            log.Debug($"{thisFileExt} in not in the list of extensions. No action taken on file {e.Name}.");
        }
    }

    private static void File_Deleted(object sender, FileSystemEventArgs e)
    {
        log.Debug($"File deleted: {e.Name} ");
    }
}
