﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;

internal static class MainWindowUIHelpers
{
    /// <summary>
    /// Gets the current theme
    /// </summary>
    /// <returns>Dark or Light</returns>
    internal static string GetSystemTheme()
    {
        BaseTheme? sysTheme = Theme.GetSystemTheme();
        return sysTheme != null ? sysTheme.ToString() : string.Empty;
    }

    /// <summary>
    /// Sets the theme
    /// </summary>
    /// <param name="mode">Light, Dark, Darker or System</param>
    internal static void SetBaseTheme(ThemeType mode)
    {
        //Retrieve the app's existing theme
        PaletteHelper paletteHelper = new();
        ITheme theme = paletteHelper.GetTheme();

        if (mode == ThemeType.System)
        {
            mode = GetSystemTheme().Equals("light") ? ThemeType.Light : ThemeType.Dark;
        }

        switch (mode)
        {
            case ThemeType.Light:
                theme.SetBaseTheme(Theme.Light);
                theme.Paper = Colors.WhiteSmoke;
                break;
            case ThemeType.Dark:
                theme.SetBaseTheme(Theme.Dark);
                break;
            case ThemeType.Darker:
                // Set card and paper background colors a bit darker
                theme.SetBaseTheme(Theme.Dark);
                theme.CardBackground = (Color)ColorConverter.ConvertFromString("#FF141414");
                theme.Paper = (Color)ColorConverter.ConvertFromString("#FF202020");
                break;
            default:
                theme.SetBaseTheme(Theme.Light);
                break;
        }

        //Change the app's current theme
        paletteHelper.SetTheme(theme);
    }

    /// <summary>
    /// Sets the MDIX primary accent color
    /// </summary>
    /// <param name="color">One of the 18 color values</param>
    internal static void SetPrimaryColor(AccentColor color)
    {
        PaletteHelper paletteHelper = new();
        ITheme theme = paletteHelper.GetTheme();
        PrimaryColor primary = color switch
        {
            AccentColor.Red => PrimaryColor.Red,
            AccentColor.Pink => PrimaryColor.Pink,
            AccentColor.Purple => PrimaryColor.Purple,
            AccentColor.DeepPurple => PrimaryColor.DeepPurple,
            AccentColor.Indigo => PrimaryColor.Indigo,
            AccentColor.Blue => PrimaryColor.Blue,
            AccentColor.LightBlue => PrimaryColor.LightBlue,
            AccentColor.Cyan => PrimaryColor.Cyan,
            AccentColor.Teal => PrimaryColor.Teal,
            AccentColor.Green => PrimaryColor.Green,
            AccentColor.LightGreen => PrimaryColor.LightGreen,
            AccentColor.Lime => PrimaryColor.Lime,
            AccentColor.Yellow => PrimaryColor.Yellow,
            AccentColor.Amber => PrimaryColor.Amber,
            AccentColor.Orange => PrimaryColor.Orange,
            AccentColor.DeepOrange => PrimaryColor.DeepOrange,
            AccentColor.Brown => PrimaryColor.Brown,
            AccentColor.Grey => PrimaryColor.Grey,
            AccentColor.BlueGray => PrimaryColor.BlueGrey,
            _ => PrimaryColor.Blue,
        };
        Color primaryColor = SwatchHelper.Lookup[(MaterialDesignColor)primary];
        theme.SetPrimaryColor(primaryColor);
        paletteHelper.SetTheme(theme);
    }

    /// <summary>
    /// Sets the value for UI scaling
    /// </summary>
    /// <param name="size">One of 5 values</param>
    /// <returns></returns>
    internal static double UIScale(MySize size)
    {
        return size switch
        {
            MySize.Smallest => 0.85,
            MySize.Smaller => 0.95,
            MySize.Default => 1.0,
            MySize.Larger => 1.05,
            MySize.Largest => 1.15,
            _ => 1.0,
        };
    }
}
