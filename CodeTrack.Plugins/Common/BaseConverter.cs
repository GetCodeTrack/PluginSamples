using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace CodeTrack.Plugins.Common
{
    public class BaseConverter<T> : MarkupExtension, IValueConverter, IMultiValueConverter where T : class, new()
    {

        public BaseConverter()
        {

        }

        private static T _Instance;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_Instance == null)
            {
                _Instance = new T();
            }
            return _Instance;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return OnConvert(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return OnConvertBack(value, targetType, parameter, culture);
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return OnConvert(values, targetType, parameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return OnConvertBack(value, targetTypes, parameter, culture);
        }


        protected virtual object OnConvert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        protected virtual object OnConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public virtual object OnConvert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public virtual object[] OnConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
