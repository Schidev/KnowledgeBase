using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UWP_PROJECT_06.Services.Converters
{
    public class SourceIdToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int sourceId = (int)value;

            return NotesService.ReadSource(sourceId).SourceName.Replace("VIDEO_", "").Replace("SOUND_", "").Replace("IMAGE_", "").Replace("DOCUMENT_", "").Replace("_", " ");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
