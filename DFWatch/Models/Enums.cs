// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch.Models;

/// <summary>
/// Navigation Page
/// </summary>
public enum NavPage
{
    None,
    Main,
    Settings,
    About,
    Exit
}

/// <summary>
/// Theme type, Light, Dark, or System
/// </summary>
public enum ThemeType
{
    Light = 0,
    Dark = 1,
    Darker = 2,
    System = 3
}

/// <summary>
/// Size of the UI, Smallest, Smaller, Default, Larger, or Largest
/// </summary>
public enum MySize
{
    Smallest = 0,
    Smaller = 1,
    Small = 2,
    Default = 3,
    Large = 4,
    Larger = 5,
    Largest = 6
}

/// <summary>
/// One of the 19 predefined Material Design in XAML colors
/// </summary>
public enum AccentColor
{
    Red = 0,
    Pink = 1,
    Purple = 2,
    [Description("Deep Purple")]
    DeepPurple = 3,
    Indigo = 4,
    Blue = 5,
    [Description("Light Blue")]
    LightBlue = 6,
    Cyan = 7,
    Teal = 8,
    Green = 9,
    [Description("Light Green")]
    LightGreen = 10,
    Lime = 11,
    Yellow = 12,
    Amber = 13,
    Orange = 14,
    [Description("Deep Orange")]
    DeepOrange = 15,
    Brown = 16,
    Grey = 17,
    [Description("Blue Gray")]
    BlueGray = 18
}
