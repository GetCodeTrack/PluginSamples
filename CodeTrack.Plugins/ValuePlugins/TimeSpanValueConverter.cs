using CodeTrack.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeTrack.Interface.Objects;
using System.ComponentModel.Composition;

namespace CodeTrack.Plugins.ValuePlugins
{
    [Export(typeof(IValuePlugin))]
    public class TimeSpanValueConverter : IValuePlugin
    {
        public string UniqueId => "272667E30FB3405FAA12AA76DDF54C6E";

        public string Title => "Timespan ValueConverter";

        public string Description => "Converts traced Timespan values to their .NET instance";

        public string Author => "Nico Van Goethem";

        public string AuthorContactInfo => "info@getcodetrack.com";

        public string IconUri => @"pack://application:,,,/CodeTrack.Plugins;Component/Images/timespan.png";

        public bool CanConvert(PointerValue value, IStore store)
        {
            if (value.Fields == null)
                return false;
            
            if (!"System.TimeSpan".Equals(value.TypeName, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

        

            return value.Fields.Any(n => "_ticks".Equals(n.Name, StringComparison.InvariantCultureIgnoreCase));

        }

        public object Convert(PointerValue value, IStore store)
        {
            var ticksField = value.Fields.FirstOrDefault(n => "_ticks".Equals(n.Name, StringComparison.InvariantCultureIgnoreCase));
            if (ticksField == null || !(ticksField.Value is Int64))
                return null;

            var ticks = (Int64)ticksField.Value;

            return new TimeSpan(ticks);
        }
    }
}
