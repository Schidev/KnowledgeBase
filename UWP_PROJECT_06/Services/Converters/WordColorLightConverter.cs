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
            List<string> colors = new List<string>
        {
            "Transparent",
            "#ffd180",
            "#e6ceff",
            "#b9f6ca",
            "#e6ceff",
            "#B2E6F3",
            "#B2E6F3",
            "#f8bbd0",
            "#fff9c4",
            "#B2E6F3",
            "#B2E6F3",
            "#B2E6F3",
            "#B2E6F3",
            "#B2E6F3",
            "#B2E6F3",
            "#B2E6F3",
            "#B2E6F3",
            "#B2E6F3",
        };


            int partOfSpeechId = (int)value;
            return colors[partOfSpeechId];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
