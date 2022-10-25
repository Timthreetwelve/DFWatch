// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;

internal static class Watch
{
    private static readonly FileSystemWatcher watcher = new();
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
        log.Info($"The file system watcher has started watching {watcher.Path}.");
    }

    public static void StopWatcher()
    {
        if (watcher != null)
        {
            watcher.EnableRaisingEvents=false;
            watcher.Dispose();
            log.Info($"The file system watcher has stopped watching {watcher.Path}.");
        }
    }
    private static void File_Renamed(object sender, RenamedEventArgs e)
    {
        log.Debug($"file renamed: {e.Name} ");
    }

    private static void File_Created(object sender, FileSystemEventArgs e)
    {
        log.Debug($"file created: {e.Name} ");
    }

    private static void File_Deleted(object sender, FileSystemEventArgs e)
    {
        log.Debug($"file deleted: {e.Name} ");
    }
}
