using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CodeTrack.Plugins.Common
{

    public class NotifyingBase : INotifyPropertyChanged
    {
        [field: NonSerialized()]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                var disp = Application.Current.Dispatcher;
                if (!disp.CheckAccess())
                {
                    disp.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        handler(this, new PropertyChangedEventArgs(propertyName));
                    }));
                }
                else
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }

        }
    }
}
