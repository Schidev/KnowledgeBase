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
    public class SourceTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int sourceTypeId = System.Convert.ToInt32(value);

            switch (sourceTypeId)
            {
                case 1:
                    return (Application.Current.Resources["colorOrangeDictionary"] as SolidColorBrush).Color.ToHex();
                case 2:
                    return (Application.Current.Resources["colorPurpleDictionary"] as SolidColorBrush).Color.ToHex();
                case 3:
                    return (Application.Current.Resources["colorGreenDictionary"] as SolidColorBrush).Color.ToHex();
                case 4:
                    return (Application.Current.Resources["colorPinkDictionary"] as SolidColorBrush).Color.ToHex();
            }

            return (Application.Current.Resources["colorPurpleLightDictionary"] as SolidColorBrush).Color.ToHex();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
