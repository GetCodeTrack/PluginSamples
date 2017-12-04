using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace CodeTrack.Plugins
{
    public class WriteLineEvent
    {
        public List<WriteEvent> Events { get; set; }
        public WriteLineEvent(params WriteEvent[] events)
        {
            if (events != null && events.Any())
            {
                Events = new List<WriteEvent>(events);
            }
            else
            {
                Events = new List<WriteEvent>();
            }
        }
    }
}
