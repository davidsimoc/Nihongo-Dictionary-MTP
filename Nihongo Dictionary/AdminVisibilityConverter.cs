using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Nihongo_Dictionary
{
    public class AdminVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Diagnostics.Debug.WriteLine($"[AdminVisibilityConverter] Received role: {value}");
            if (value is string role && role.ToLower() == "admin")
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}