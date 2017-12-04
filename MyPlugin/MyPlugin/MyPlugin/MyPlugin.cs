using CodeTrack.Interface;
using MyNamespace.Commands;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;

namespace MyNamespace
{
    [Export(typeof(IViewPlugin))]
    public class MyPlugin : ViewPluginBase
    {
        public override string UniqueId => "C88E8D6FD091402F94BFCB2220FC36BD";

        public override string Author => "Your name here";
        public override string AuthorContactInfo => "Your contact info here";

        public override string Description => "Some info about your plugin";

        public override string Title => "My plugin";

        public override string IconUri => @"pack://application:,,,/MyPlugin.Plugins;Component/Images/cool.png";

        public override bool IsCompatibleWith(IMethodRuleSet rulesets)
        {
            return true;
        }

        public ICommand ClickMeCommand => new GenericCommand<MyPlugin>(this, (p, a) => { MessageBox.Show("Hello world !"); });

    }
}
