using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DrinkWater
{
    public class VisibleWhenZeroConverter : IValueConverter
    {
        public object Convert(object value,
            Type targetType, object parameter, string language)
        {
            bool isVisible = (bool)value;
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value,
               Type targetType, object parameter, string language)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }
}
