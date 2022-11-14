// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;

/// <summary>Routed UI Commands</summary>
/// <remarks>Commands for Show, Start, Stop, and Exit</remarks>
public static class CustomCommands
{
    public static RoutedUICommand Exit { get; } =
        new("E_xit", "Exit", typeof(CustomCommands));

    public static RoutedUICommand ShowMainWindow { get; } =
        new("_Show Window", "Show", typeof(CustomCommands));

    public static RoutedUICommand StopWatching { get; } =
    new("Sto_p Watcher", "StopWatching", typeof(CustomCommands));

    public static RoutedUICommand StartWatching { get; } =
    new("Star_t Watcher", "StartWatching", typeof(CustomCommands));
}
