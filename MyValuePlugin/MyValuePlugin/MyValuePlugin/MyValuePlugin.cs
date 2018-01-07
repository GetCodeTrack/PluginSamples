using CodeTrack.Interface;
using System;
using System.Linq;
using CodeTrack.Interface.Objects;
using System.ComponentModel.Composition;

namespace MyNamespace
{
    [Export(typeof(IValuePlugin))]
    public class MyValuePlugin : IValuePlugin
    {
        public string UniqueId => "19350860169E4245B92708770EC935FC";

        public string Title => "My plugin";

        public string Description => "Some info about your plugin";

        public string Author => "Your name here";

        public string AuthorContactInfo => "Your contact info here";

        public string IconUri => @"pack://application:,,,/MyValuePlugin.Plugins;Component/Images/cool.png";

        public bool CanConvert(PointerValue value, IStore store)
        {
            // THIS IS JUST AN EXAMPLE IMPLEMENTATION, REPLACE BY YOUR OWN
            if (value.Fields == null)
                return false;

            if (!"System.Uri".Equals(value.TypeName, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return value.Fields.Any(n => "m_String".Equals(n.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        public object Convert(PointerValue value, IStore store)
        {
            // THIS IS JUST AN EXAMPLE IMPLEMENTATION, REPLACE BY YOUR OWN
            var stringField = value.Fields.FirstOrDefault(n => "m_String".Equals(n.Name, StringComparison.InvariantCultureIgnoreCase));
            if (stringField == null || !(stringField.Value is string))
                return null;
        
            return stringField.Value;
        }
    }
}
