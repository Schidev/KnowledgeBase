using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UWP_PROJECT_06.Services.Converters
{
    public class WordColorAccentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            List<string> colors = new List<string>
        {
            "Transparent",
            "#ffb74d",
            "#D7A3EB",
            "#6AD196",
            "#D7A3EB",
            "#5BBCEE",
            "#5BBCEE",
            "#F292AC",
            "#F0E98E",
            "#5BBCEE",
            "#5BBCEE",
            "#5BBCEE",
            "#5BBCEE",
            "#5BBCEE",
            "#5BBCEE",
            "#5BBCEE",
            "#5BBCEE",
            "#5BBCEE",
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
