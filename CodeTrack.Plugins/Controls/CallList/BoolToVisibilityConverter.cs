using CodeTrack.Plugins.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CodeTrack.Plugins.Controls
{
    public class BoolToVisibilityConverter : BaseConverter<BoolToVisibilityConverter>
    {
        public BoolToVisibilityConverter()
        {

        }
        protected override object OnConvert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool invert;
            bool.TryParse((string)parameter, out invert);
            if (invert)
            {
                return (bool)value ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            }


        }
    }
}
