// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;
internal static class Files
{
    #region NLog Instance
    private static readonly Logger log = LogManager.GetLogger("logTemp");
    #endregion NLog Instance

    #region Check for locks on file to be moved
    private static async Task<bool> CheckForLocksAsync(FileInfo file)
    {
        log.Debug($"Checking for locks on {file.Name}.");
        // Sleep a bit before checking to avoid file not found message
        await Task.Delay(UserSettings.Setting.InitialDelay);
        for (int i = 0; i < UserSettings.Setting.NumRetries; i++)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                { }
                if (i == 0)
                {
                    log.Debug($"No locks detected on file {file.Name}.");
                }
                else
                {
                    log.Info($"Obtained exclusive lock on file {file.Name} on attempt {i + 1}.");
                }
                return true;
            }
            catch (IOException ex)
            {
                log.Warn($"{ex.Message}");
                log.Debug("Sleeping 1000 milliseconds.");
                await Task.Delay(1000);
            }
        }
        log.Warn($"Failed to get exclusive lock on {file} after {UserSettings.Setting.NumRetries} attempts.");
        return false;
    }
    #endregion Check for locks on file to be moved

    #region Move the file
    public static async void MoveFile(FileInfo file)
    {
        if (!File.Exists(file.FullName))
        {
            return;
        }

        string destinationFile = Path.Combine(UserSettings.Setting.DesitinationFolder, file.Name);

        if (File.Exists(destinationFile))
        {
            log.Error($"Failed to move {file.Name} because {destinationFile} already exists.");
            return;
        }

        if (await CheckForLocksAsync(file))
        {
            try
            {
                File.Move(file.FullName, destinationFile);
                log.Info($"Moved {file.Name} to {destinationFile}.");
            }
            catch (IOException) when (File.Exists(destinationFile))
            {
                log.Error($"Failed to move {file.Name} because {destinationFile} already exists.");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Failed to move {file.Name} to {destinationFile}.");
            }
        }
    }
    #endregion Move the file

    #region Is newly created file's extension in the list?
    public static bool CheckExtension(ObservableCollection<string> extlist, string extension)
    {
        foreach (string ext in extlist)
        {
            if (LikeOperator.LikeString(extension, ext, CompareMethod.Text))
            {
                return true;
            }
        }
        return false;
    }
    #endregion Is newly created file's extension in the list?
}
