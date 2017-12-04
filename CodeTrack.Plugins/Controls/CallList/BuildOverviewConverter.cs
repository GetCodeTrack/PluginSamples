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

namespace CodeTrack.Plugins.Controls
{
    public class BuildOverviewConverter : BaseConverter<BuildOverviewConverter>
    {

        public override object OnConvert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var doc = values[0] as IStoreViewDocument;
            if (doc == null)
                return Binding.DoNothing;

            var call = (MethodCallSmallInfo)values[1]  ;
          

            var ret = doc.BuildOverViewPanel(call.Ptr);
          
            ret.Height = 120;
         

            return ret;

        }
        //public override object OnConvert(object[] values, Type targetType, object parameter, CultureInfo culture)
        //{
        //    var doc = values[0] as IStoreViewDocument;
        //    if (doc == null)
        //        return null;

        //    var call = values[1] as MethodCallInfo;
        //    Border b;
        //    var container = values[2] as FrameworkElement;

        //    var ret = doc.BuildOverViewPanel(call.Ptr);
        //    var w = container == null ? 100 : Math.Max(100, container.ActualWidth - 50);
        //    ret.Height = 120;
        //    ret.Width = w;


        //    var size = new Size(ret.Width, ret.Height);
        //    ret.Measure(size);
        //    ret.Arrange(new Rect(size));
        //    RenderTargetBitmap bmp = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);

        //    bmp.Render(ret);


        //    var bitmapImage = new BitmapImage();
        //    var bitmapEncoder = new PngBitmapEncoder();
        //    bitmapEncoder.Frames.Add(BitmapFrame.Create(bmp));

        //    using (var stream = new MemoryStream())
        //    {
        //        bitmapEncoder.Save(stream);
        //        stream.Seek(0, SeekOrigin.Begin);

        //        bitmapImage.BeginInit();
        //        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmapImage.StreamSource = stream;
        //        bitmapImage.EndInit();
        //    }

        //    return bitmapImage;

        //}
    }
}
