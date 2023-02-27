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
    public class SettingsHotkeyButtonSaveBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool IsActive = (bool)value;

            return IsActive
                ? (Application.Current.Resources["colorWhite"] as SolidColorBrush).Color.ToHex()
                : (Application.Current.Resources["RedControlColor"] as SolidColorBrush).Color.ToHex();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
