using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UWP_PROJECT_06.Services.Converters
{
    public class WordColorLightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int partOfSpeechId = (int)value;
            return SettingsService.ReadColor(partOfSpeechId);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
