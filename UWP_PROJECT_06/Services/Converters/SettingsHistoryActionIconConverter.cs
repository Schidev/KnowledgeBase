using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UWP_PROJECT_06.Services.Converters
{
    public class SettingsHistoryActionIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var action = value as string;

            switch (action)
            {
                case "Created":
                    return "\uECC8";
                case "Read":
                    return "\uE8E5";
                case "Updated":
                    return "\uE777";
                case "Deleted":
                    return "\uECC9";
            }

            return "\uF19D";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
