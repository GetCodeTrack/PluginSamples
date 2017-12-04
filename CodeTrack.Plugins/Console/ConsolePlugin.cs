using CodeTrack.Interface.Objects;
using CodeTrack.Interface;
using CodeTrack.Plugins.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace CodeTrack.Plugins
{
    [Export(typeof(IViewPlugin))]
    public class ConsolePlugin : NotifyingBase, IViewPlugin
    {
        public string UniqueId => "557D4067BE5647099FE54FF7D862289A";
        public const string MethodRuleSetGUID = "00a554f361594fe986d8e671267cc045";

        public string Title => "Console";
        public string Description => "Console events";
        public string Author => "Nico Van Goethem";
        public string AuthorContactInfo => "info@getcodetrack.com";


        public string IconUri => @"pack://application:,,,/CodeTrack.Plugins;Component/Images/appbar.console.png";
        public bool IsCompatibleWith(IMethodRuleSet ruleset)
        {
            return ruleset.Guid == MethodRuleSetGUID;
        }

        private object _DataContext;
        public object DataContext
        {
            get { return _DataContext; }
            set
            {
                _DataContext = value;
                ResetDocAndStore(_DataContext as IStoreViewDocument);
            }
        }

        private IStoreViewDocument _Doc;
        private IStore _Store;

        private ICodeTrackApplication  _CodeTrackApplication;
        public const string Setting_ConsoleWritingClasses = "Console writing classes";
        public ConsolePlugin()
        {
            _CodeTrackApplication = ((ICodeTrackApplication)Application.Current);
            _CodeTrackApplication.EnsureSetting(this, Setting_ConsoleWritingClasses, 
                "Semicolon separated list of all classes that need to be shown in the console output.\nThe classes should be specified as regular expressions."
                , "System\\.Console;NLog\\..*");
        }

      

        private void ResetDocAndStore(IStoreViewDocument doc)
        {
            _Doc = doc;

            if (_Store != null)
            {
                _Store.PropertyChanged -= _Store_PropertyChanged;
            }

            _Store = doc?.Store;

            if (_Store != null)
            {
                _Store.PropertyChanged += _Store_PropertyChanged;
            }

            UpdateItems();
        }

        private void _Store_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsInvalid")
            {
                UpdateItems();
            }
        }


        private async void UpdateItems()
        {
            Items = new List<WriteLineEvent>();
            Waiting = true;

            await Task.Run(() => Items = GetItems());

            Waiting = false;

        }

        public static Dictionary<int, Brush> _Colors = new Dictionary<int, Brush>()
        {
            {(int)ConsoleColor.Black, Brushes.Black },
            {(int)ConsoleColor.Blue, Brushes.Blue },
            {(int)ConsoleColor.Cyan, Brushes.Cyan },
            {(int)ConsoleColor.DarkBlue, Brushes.DarkBlue },
            {(int)ConsoleColor.DarkCyan, Brushes.DarkCyan },
            {(int)ConsoleColor.DarkGray, Brushes.DarkGray },
            {(int)ConsoleColor.DarkGreen, Brushes.DarkGreen },
            {(int)ConsoleColor.DarkMagenta, Brushes.DarkMagenta },
            {(int)ConsoleColor.DarkRed, Brushes.DarkRed },
            {(int)ConsoleColor.DarkYellow, new SolidColorBrush(Color.FromRgb(0x80,0x80,0x00)) },
            {(int)ConsoleColor.Gray, Brushes.Gray },
            {(int)ConsoleColor.Green, Brushes.Green },
            {(int)ConsoleColor.Magenta, Brushes.Magenta },
            {(int)ConsoleColor.Red, Brushes.Red },
            {(int)ConsoleColor.White, Brushes.White },
            {(int)ConsoleColor.Yellow, Brushes.Yellow },


        };

        private List<WriteLineEvent> GetItems()
        {
            try
            {

                if (_Doc == null)
                    return null;

                if (_Store == null || _Store.IsInvalid)
                    return null;

                var allConsole = _Store.IterateCallsWithMethodRuleSet(ConsolePlugin.MethodRuleSetGUID).Select(ptr => _Store.GetMethodCallInfo(ptr)).ToList();


                var onclickCommand = new GenericCommand<object>(null, (s, p) =>
                {
                    _Doc.ShowTimelinePanel();
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        _Store.SetSelectedCalls(new[] { ((WriteEvent)p).CallPtr }, this);
                    }));

                });


                var methods = new[] { "Write", "WriteLine", "set_ForegroundColor", "set_BackgroundColor", "ResetColor" };
                var consoleWritingClasses = _CodeTrackApplication.GetPluginSetting<string>(this, Setting_ConsoleWritingClasses)
                                                                 .Split(';')
                                                                 .Select(n=>new Regex(n,RegexOptions.CultureInvariant|RegexOptions.IgnoreCase|RegexOptions.Compiled))
                                                                 .ToArray(); 

                var matches = allConsole.AsParallel().Where(n => n.Name == "ResetColor" || n.Name == "WriteLine" || (n.HasTracedParameters && methods.Contains(n.Name) && n.Parameters.Select(p => p.Value).FirstOrDefault() != null))
                    .Where(item =>
                    {
                        if (item.Class != "System.Console")
                        {
                            var stack = _Store.GetCallStack(item.Ptr);
                            if (!stack.Any(n =>
                            {
                                var cname = _Store.GetCallClassName(n);
                                return consoleWritingClasses.Any(x=> x.IsMatch(cname));
                            }
                            ))
                            {
                                return false;
                            }
                        }
                        return true;
                    })
                    .OrderBy(n => n.StartValue).ToList();
                var ret = new List<WriteLineEvent>();

                Brush foreground = Brushes.White;
                Brush background = Brushes.Black;
                WriteLineEvent writeLineEvent = new WriteLineEvent();


                foreach (var item in matches)
                {

                    switch (item.Name)
                    {
                        case "Write":
                            var str = item.Parameters.Select(p => p.Value).Select(p => p.ToString()).FirstOrDefault();
                            writeLineEvent.Events.Add(new WriteEvent() { Text = str, CallPtr = item.Ptr, OnSelect = onclickCommand, Foreground = foreground, Background = background });

                            break;
                        case "WriteLine":
                            if (writeLineEvent.Events.Any())
                            {
                                ret.Add(writeLineEvent);
                            }
                            ret.Add(new WriteLineEvent(new WriteEvent() { Text = item.HasTracedParameters ? item.Parameters.Select(p => p.Value).Select(p => p.ToString()).FirstOrDefault() : "", CallPtr = item.Ptr, OnSelect = onclickCommand, Foreground = foreground, Background = background }));
                            writeLineEvent = new WriteLineEvent();
                            break;
                        case "set_ForegroundColor":
                            var colorStrForeground = (int)(item.Parameters.Select(p => p.Value.Value).FirstOrDefault());
                            foreground = _Colors[colorStrForeground];
                            break;
                        case "set_BackgroundColor":
                            var colorStrBackground = (int)(item.Parameters.Select(p => p.Value.Value).FirstOrDefault());
                            background = _Colors[colorStrBackground];
                            break;
                        case "ResetColor":
                            foreground = Brushes.White;
                            background = Brushes.Black;
                            break;
                    }


                }
                if (writeLineEvent.Events.Any())
                {
                    ret.Add(writeLineEvent);
                }

                return ret;
            }
            finally
            {
                Waiting = false;
            }
        }

        bool _Waiting;
        public bool Waiting
        {
            get { return _Waiting; }
            set
            {
                if (value == _Waiting)
                    return;
                _Waiting = value;
                OnPropertyChanged("Waiting");
            }
        }



        List<WriteLineEvent> _Items;
        public List<WriteLineEvent> Items
        {
            get { return _Items; }
            set
            {
                if (value == _Items)
                    return;
                _Items = value;
                OnPropertyChanged("Items");
            }
        }


    }
}
