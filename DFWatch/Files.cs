// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;
internal static class Files
{
    #region NLog Instance
    private static readonly Logger _log = LogManager.GetLogger("logTemp");
    #endregion NLog Instance

    #region Regex Instance
    private static readonly Regex _regex = new(@"^(.+) \((\d+)\)$");
    #endregion Regex Instance

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

    #region Check for locks on file to be moved
    /// <summary>Checks for locks on the file.</summary>
    /// <param name="file">The file.</param>
    /// <returns>true if file isn't locked, otherwise false</returns>
    private static async Task<bool> CheckForLocksAsync(FileInfo file)
    {
        _log.Debug($"Checking for locks on {file.Name}.");
        // Delay a bit before checking to avoid file not found message
        await Task.Delay(UserSettings.Setting.InitialDelay);
        for (int i = 0; i < UserSettings.Setting.NumRetries; i++)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                { }
                if (i == 0)
                {
                    _log.Debug($"No locks detected on file {file.Name}.");
                }
                else
                {
                    _log.Info($"Obtained exclusive lock on file {file.Name} on attempt {i + 1}.");
                }
                return true;
            }
            catch (IOException ex)
            {
                if (ex.Message.Contains("being used by another process"))
                {
                    _log.Warn($"{file.Name} cannot be moved. It is being used by another process.");
                    _log.Debug($"Waiting {UserSettings.Setting.RetryDelay} milliseconds before trying again.");
                    await Task.Delay(UserSettings.Setting.RetryDelay);
                }
                else if (ex.Message.Contains("Could not find file"))
                {
                    _log.Warn($"{file.Name} was not found.");
                    return false;
                }
                else
                {
                    _log.Warn($"{ex.Message}");
                    _log.Debug($"Waiting {UserSettings.Setting.RetryDelay} milliseconds before trying again.");
                    await Task.Delay(UserSettings.Setting.RetryDelay);
                }
            }
            catch (Exception ex)
            {
                _log.Warn($"{ex.Message}");
                return false;
            }
        }
        _log.Warn($"Failed to get exclusive lock on {file} after {UserSettings.Setting.NumRetries} attempts.");
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

        // Rename if destination file already exists
        if (File.Exists(destinationFile) && UserSettings.Setting.RenameIfDuplicate)
        {
            _log.Debug($"{destinationFile} already exists. Will attempt to rename.");
            destinationFile = CreateUniqueFileName(destinationFile);
        }
        // If rename option is false write a log message and return
        else if(File.Exists(destinationFile) && !UserSettings.Setting.RenameIfDuplicate)
        {
            _log.Warn($"{destinationFile} already exists. Option to rename is false.");
            return;
        }

        // Check for locks on the file and if there are none attempt to move the file to the destination folder
        if (await CheckForLocksAsync(file))
        {
            try
            {
                File.Move(file.FullName, destinationFile);
                _log.Info($"Moved {file.Name} to {destinationFile}.");
            }
            catch (IOException) when (File.Exists(destinationFile))
            {
                _log.Error($"Failed to move {file.Name} because {destinationFile} already exists.");
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Failed to move {file.Name} to {destinationFile}.");
            }
        }
    }
    #endregion Move the file

    #region Create unique file name
    /// <summary>If file exists, create a new filename by appending an integer inside parenthesis.</summary>
    /// <remarks>Adapted from: https://stackoverflow.com/a/22373595/15237757</remarks>
    /// <param name="filePath">Path of destination file.</param>
    /// <returns>A unique filename including path.</returns>
    public static string CreateUniqueFileName(string filePath)
    {
        if (File.Exists(filePath))
        {
            string folderPath = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fileExtension = Path.GetExtension(filePath);
            string newFileName;
            int number = 0;

            // If the original filename already has a number in parenthesis (in the same position)
            // grab that number which will be incremented in the next step.
            if (_regex.Match(fileName).Success)
            {
                number = int.Parse(_regex.Match(fileName).Groups[2].Value);
                fileName = _regex.Match(fileName).Groups[1].Value;
            }

            // Increment the number and insert it (with a leading space) in front of the extension.
            do
            {
                number++;
                newFileName = $"{fileName} ({number}){fileExtension}";
                filePath = Path.Combine(folderPath, newFileName);
            }
            while (File.Exists(filePath));
            _log.Debug($"{fileName}{fileExtension} will be renamed to {newFileName}");
        }
        return filePath;
    }
    #endregion Create unique file name
}
