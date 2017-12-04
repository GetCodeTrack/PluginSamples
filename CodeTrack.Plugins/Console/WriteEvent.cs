using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace CodeTrack.Plugins
{
    public class WriteEvent
    {
        public string Text { get; set; }
        public long CallPtr { get; set; }

        public ICommand OnSelect { get; set; }

        public Brush Foreground { get; set; }
        public Brush Background { get; set; }

    }
}
