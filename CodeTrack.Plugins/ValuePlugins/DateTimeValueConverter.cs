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
    public class DateTimeValueConverter : IValuePlugin
    {
        public string UniqueId => "E98A4611980E4F6D8E8D230F1AF6C6D6";
        public string Title => "DateTime ValueConverter";

        public string Description => "Converts traced DateTime values to their .NET instance";

        public string Author => "Nico Van Goethem";

        public string AuthorContactInfo => "info@getcodetrack.com";

        public string IconUri => @"pack://application:,,,/CodeTrack.Plugins;Component/Images/datetime.png";

        public bool CanConvert(PointerValue value, IStore store)
        {
            if (value.Fields == null)
                return false;

            if (!"System.DateTime".Equals(value.TypeName, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

        

            return value.Fields.Any(n => "dateData".Equals(n.Name, StringComparison.InvariantCultureIgnoreCase));

        }

        public object Convert(PointerValue value, IStore store)
        {
            var dataField = value.Fields.FirstOrDefault(n => "dateData".Equals(n.Name, StringComparison.InvariantCultureIgnoreCase));
            if (dataField == null || !(dataField.Value is UInt64))
                return null;

            var dateData = (UInt64)dataField.Value;


            var ticks = (long)(dateData & 4611686018427387903uL);

            var internalKind = dateData & 13835058055282163712uL;

            DateTimeKind kind;
            if (internalKind == 0uL)
            {
                kind= DateTimeKind.Unspecified;
            }
            if (internalKind != 4611686018427387904uL)
            {
                kind= DateTimeKind.Local;
            }
            kind= DateTimeKind.Utc;

            return new DateTime(ticks, kind);
        }
    }
}
