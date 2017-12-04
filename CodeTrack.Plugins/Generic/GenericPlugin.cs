using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using CodeTrack.Interface;
using System.Windows.Controls;

namespace CodeTrack.Plugins
{
    [Export(typeof(IViewPlugin))]
    public class GenericPlugin : ViewPluginBase
    {
        public override string UniqueId => "5EE5F87D62254AEBAD5F118BCCD4BC0B";
        public override string Title => "Generic";
        public override string Description => "Overview of all events";

        public override string Author => "Nico Van Goethem";
        public override string AuthorContactInfo => "info@getcodetrack.com";

        public override string IconUri => @"pack://application:,,,/CodeTrack.Plugins;Component/Images/layers.png";

        public override bool IsCompatibleWith(IMethodRuleSet ruleset)
        {
            return true;
        }
    }
}
