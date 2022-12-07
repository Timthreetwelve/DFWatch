// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;
internal static class Files
{
    #region NLog Instance
    private static readonly Logger log = LogManager.GetLogger("logTemp");
    #endregion NLog Instance

    #region Check for locks on file to be moved
    /// <summary>Checks for locks on the file.</summary>
    /// <param name="file">The file.</param>
    /// <returns>true if file isn't locked, otherwise false</returns>
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
                if (ex.Message.Contains("being used by another process"))
                {
                    log.Warn($"{file.Name} cannot be moved. It is being used by another process.");
                    log.Debug($"Waiting {UserSettings.Setting.RetryDelay} milliseconds before trying again.");
                    await Task.Delay(UserSettings.Setting.RetryDelay);
                }
                else if (ex.Message.Contains("Could not find file"))
                {
                    log.Warn($"{file.Name} was not found.");
                    return false;
                }
                else
                {
                    log.Warn($"{ex.Message}");
                    log.Debug($"Waiting {UserSettings.Setting.RetryDelay} milliseconds before trying again.");
                    await Task.Delay(UserSettings.Setting.RetryDelay);
                }
            }
            catch (Exception ex)
            {
                log.Warn($"{ex.Message}");
                return false;
            }
        }
        log.Warn($"Failed to get exclusive lock on {file} after {UserSettings.Setting.NumRetries} attempts.");
        return false;
    }
    #endregion Check for locks on file to be moved

    #region Move the file
    /// <summary>Moves the file.</summary>
    /// <param name="file">The file.</param>
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
    /// <summary>Checks to see if the extension is in the list</summary>
    /// <param name="extlist">The extension list.</param>
    /// <param name="extension">The extension.</param>
    /// <returns>true if the extension is in the list.</returns>
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
