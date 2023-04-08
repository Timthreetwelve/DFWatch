// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch.ViewModels;

internal partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private static bool _uIExpanderOpen;

    [ObservableProperty]
    private static bool _extExpanderOpen;

    [ObservableProperty]
    private static bool _folderExpanderOpen;

    [ObservableProperty]
    private static bool _appExpanderOpen;
}
