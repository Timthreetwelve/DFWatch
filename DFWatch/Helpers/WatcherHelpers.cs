// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch.Helpers;

#pragma warning disable RCS1102 // Make class static.
public class WatcherHelpers
#pragma warning restore RCS1102 // Make class static.
{
    public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

    private static bool _isWatching;
    public static bool IsWatching
    {
        get => _isWatching;

        set
        {
            if (_isWatching != value)
            {
                _isWatching = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(IsWatching)));
            }
        }
    }
}
