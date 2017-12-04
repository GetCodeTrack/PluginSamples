using CodeTrack.Plugins.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CodeTrack.Plugins
{
    public class NullToVisibilityConverter : BaseConverter<NullToVisibilityConverter>
    {
        public NullToVisibilityConverter()
        {

        }
        protected override object OnConvert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool invert;
            bool.TryParse((string)parameter, out invert);
            if (invert)
            {
                return value == null ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return value == null ? Visibility.Collapsed : Visibility.Visible;
            }


        }
    }
}
