using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace UWP_PROJECT_06.Services.Converters
{
    public class SettingsHistoryActionForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var action = value as string;

            switch (action)
            {
                case "Created":
                    return (Application.Current.Resources["GreenControlColor"] as SolidColorBrush).Color.ToHex();
                case "Read":
                    return (Application.Current.Resources["BlueControlColor"] as SolidColorBrush).Color.ToHex();
                case "Updated":
                    return (Application.Current.Resources["OrangeControlColor"] as SolidColorBrush).Color.ToHex();
                case "Deleted":
                    return (Application.Current.Resources["RedControlColor"] as SolidColorBrush).Color.ToHex();
            }

            return (Application.Current.Resources["DimGreyControlColor"] as SolidColorBrush).Color.ToHex();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
