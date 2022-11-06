// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;

internal class ColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        PaletteHelper paletteHelper = new();
        ITheme theme = paletteHelper.GetTheme();
        BaseTheme baseTheme = theme.GetBaseTheme();

        if (value != null)
        {
            if (baseTheme == BaseTheme.Light)
            {
                if (UserSettings.Setting.ColoredMessages)
                {
                    if (value.ToString().IndexOf("Heartbeat") == 29)
                    {
                        return Brushes.SlateBlue;
                    }
                    else if (value.ToString().IndexOf("ERR") == 24)
                    {
                        return Brushes.Red;
                    }
                    else if (value.ToString().IndexOf("WRN") == 24)
                    {
                        return Brushes.OrangeRed;
                    }
                    else if (value.ToString().IndexOf("INF") == 24)
                    {
                        return Brushes.DarkGreen;
                    }
                    else if (value.ToString().IndexOf("DBG") == 24)
                    {
                        return Brushes.DimGray;
                    }
                    else
                    {
                        return Brushes.Black;
                    }
                }
                else
                {
                    return Brushes.Black;
                }
            }
            else if (baseTheme == BaseTheme.Dark)
            {
                if (UserSettings.Setting.ColoredMessages)
                {
                    if (value.ToString().IndexOf("Heartbeat") == 29)
                    {
                        return Brushes.LightSlateGray;
                    }
                    else if (value.ToString().IndexOf("ERR") == 24)
                    {
                        return Brushes.Red;
                    }
                    else if (value.ToString().IndexOf("WRN") == 24)
                    {
                        return Brushes.Orange;
                    }
                    else if (value.ToString().IndexOf("INF") == 24)
                    {
                        return Brushes.MediumSpringGreen;
                    }
                    else if (value.ToString().IndexOf("DBG") == 24)
                    {
                        return Brushes.SkyBlue;
                    }
                    else
                    {
                        return Brushes.WhiteSmoke;
                    }
                }
                else
                {
                    return Brushes.WhiteSmoke;
                }
            }
        }
        return Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
