// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;

internal static class DialogHelpers
{
    /// <summary>
    /// Shows the About dialog.
    /// </summary>
    internal static async void ShowAboutDialog()
    {
        About about = new();
        _ = await DialogHost.Show(about, "MainDialogHost");
    }
}
