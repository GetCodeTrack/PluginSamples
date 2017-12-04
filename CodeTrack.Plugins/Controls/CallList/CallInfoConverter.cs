using CodeTrack.Plugins.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using CodeTrack.Interface;
using CodeTrack.Interface.Objects;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Runtime.ExceptionServices;

namespace CodeTrack.Plugins.Controls
{
    public class CallInfoConverter : BaseConverter<CallInfoConverter>
    {

        [HandleProcessCorruptedStateExceptions]
        public override object OnConvert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var call = (long)values[0];
            var store = values[1] as IStore;
            
            if (store == null)
                return Binding.DoNothing;

            return store.GetMethodCallInfo(call);
        }
      
    }
}
