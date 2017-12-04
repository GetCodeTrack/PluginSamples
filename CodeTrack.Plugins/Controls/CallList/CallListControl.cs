using CodeTrack.Interface.Objects;
using CodeTrack.Interface;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.ComponentModel;
using CodeTrack.Plugins.Common;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Runtime.ExceptionServices;

namespace CodeTrack.Plugins.Controls
{
    public class CallListControl : Control
    {
        static CallListControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CallListControl), new FrameworkPropertyMetadata(typeof(CallListControl)));
        }
        public CallListControl()
        {
            GotoLocationCommand = new GenericCommand<CallListControl>(this, (s, p) =>
             {

                 s.Document.ShowTimelinePanel();
                 Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                 {
                     s.Store.SetSelectedCalls(new[] { ((MethodCallInfo)p).Ptr }, this);
                 }));

             });
            IsVisibleChanged += CallListControl_IsVisibleChanged;

            var filterEvents = this.ToObservable<string>(FilterTextProperty);
            var sliced = filterEvents.Throttle(TimeSpan.FromMilliseconds(300));
            sliced.Subscribe(a => Dispatcher.BeginInvoke(new Action(InvalidateFilter), DispatcherPriority.Send));

        }

        private async void InvalidateFilter()
        {
            await ReSetCalls();
        }

        private void CallListControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                if (_ListBox != null && SelectedCall.HasValue && IsVisible)
                {
                    _ListBox.ScrollIntoView(SelectedCall);
                }
            }));
        }

        public IStoreViewDocument Document
        {
            get { return (IStoreViewDocument)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Document.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(IStoreViewDocument), typeof(CallListControl), new PropertyMetadata(null));



        public IStore Store
        {
            get { return (IStore)GetValue(StoreProperty); }
            set { SetValue(StoreProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Store.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StoreProperty =
            DependencyProperty.Register("Store", typeof(IStore), typeof(CallListControl), new PropertyMetadata(null, OnStorePropertyChanged));

        private async static void OnStorePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var clc = (CallListControl)d;

            var oldStore = e.OldValue as IStore;
            var newStore = e.NewValue as IStore;

            if (oldStore != null)
            {
                oldStore.PropertyChanged -= clc.Store_PropertyChanged;
            }
            if (newStore != null)
            {
                newStore.PropertyChanged += clc.Store_PropertyChanged;
            }

            await clc.ReSetCalls();
        }

        private ListBox _ListBox;
        public override void OnApplyTemplate()
        {
            _ListBox = GetTemplateChild("PART_ListBox") as ListBox;
        }


        private void Store_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedCalls")
            {
                var selection = Store.GetSelectedCalls().FirstOrDefault();
                SetCurrentValue(SelectedCallProperty, selection);




            }
        }

        public bool StoreIsInvalid
        {
            get { return (bool)GetValue(StoreIsInvalidProperty); }
            set { SetValue(StoreIsInvalidProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StoreIsInvalid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StoreIsInvalidProperty =
            DependencyProperty.Register("StoreIsInvalid", typeof(bool), typeof(CallListControl), new PropertyMetadata(false, OnStoreIsInvalidPropertyChanged));

        private async static void OnStoreIsInvalidPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var clc = (CallListControl)d;
            await clc.ReSetCalls();
        }

        //private async void Store_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "IsInvalid")
        //    {
        //        await ReSetCalls();
        //    }
        //}



        public string FilterText
        {
            get { return (string)GetValue(FilterTextProperty); }
            set { SetValue(FilterTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilterText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterTextProperty =
            DependencyProperty.Register("FilterText", typeof(string), typeof(CallListControl), new PropertyMetadata(""));


        private async Task<bool> ReSetCalls()
        {
            var selection = SelectedCall;

            if (Store == null || Store.IsInvalid)
            {
                SetCurrentValue(CallsProperty, new List<long>());
            }
            else
            {
                SetCurrentValue(CallsProperty, new List<long>());
                SetCurrentValue(WaitingProperty, true);
                try
                {
                    var calls = await GetCalls();
                    if (!calls.Any())
                        calls = null;
                    SetCurrentValue(CallsProperty, calls);
                    SetCurrentValue(SelectedCallProperty, selection);

                }
                finally
                {
                    SetCurrentValue(WaitingProperty, false);
                }
            }
            return true;
        }
        public bool Waiting
        {
            get { return (bool)GetValue(WaitingProperty); }
            set { SetValue(WaitingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Waiting.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaitingProperty =
            DependencyProperty.Register("Waiting", typeof(bool), typeof(CallListControl), new PropertyMetadata(false));

        private Task<IEnumerable<long>> GetCalls(bool filterWithRegex = false)
        {

            IStore s = Store;
            Regex r = null;
            bool filter = !string.IsNullOrEmpty(FilterText);
            if (filter && filterWithRegex)
            {

                try
                {
                    var str = /*"^" +*/ FilterText.Replace("(", "\\(").Replace(")", "\\)").Replace(".", "\\.").Replace("*", ".*") /*+ "$"*/;
                    r = new Regex(str, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }
                catch
                {

                }
            }

            if (!filter)
            {
                var ret = Task.Run(() => (IEnumerable<long>)s.IterateCallsWithMethodRuleSet().ToList());
                return ret;
            }
            else
            {
                if (filterWithRegex)
                {
                    var ret = Task.Run(() => (IEnumerable<long>)s.IterateCallsWithMethodRuleSet().AsParallel().Where(n =>  r.IsMatch(s.GetCallFullMethodName(n))).ToList());
                    return ret;
                }
                else
                {
                    var fText = FilterText.Trim().ToUpperInvariant();
                    var ret = Task.Run(() => (IEnumerable<long>)s.IterateCallsWithMethodRuleSet().AsParallel().Where(n => s.GetCallFullMethodName(n).ToUpperInvariant().Contains(fText)).ToList());
                    return ret;
                }
               
            }

        }



        public long? SelectedCall
        {
            get { return (long?)GetValue(SelectedCallProperty); }
            set { SetValue(SelectedCallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedCall.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedCallProperty =
            DependencyProperty.Register("SelectedCall", typeof(long?), typeof(CallListControl), new PropertyMetadata(null, OnSelectedCallPropertyChanged));

        private static void OnSelectedCallPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var clc = (CallListControl)d;
            if (clc.SelectedCall.HasValue && e.OldValue != e.NewValue)
            {
                clc.Store.TryUpdateInfoTreeItemForCall(clc.SelectedCall.Value);

            }

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                if (clc._ListBox != null && clc.SelectedCall.HasValue && clc.IsVisible)
                {
                    clc._ListBox.ScrollIntoView(clc.SelectedCall);
                }
            }));
        }

        public ICommand GotoLocationCommand
        {
            get { return (ICommand)GetValue(GotoLocationCommandProperty); }
            set { SetValue(GotoLocationCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GotoLocationCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GotoLocationCommandProperty =
            DependencyProperty.Register("GotoLocationCommand", typeof(ICommand), typeof(CallListControl), new PropertyMetadata(null));


        public IEnumerable<long> Calls
        {
            get { return (IEnumerable<long>)GetValue(CallsProperty); }
            set { SetValue(CallsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Calls.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallsProperty =
            DependencyProperty.Register("Calls", typeof(IEnumerable<long>), typeof(CallListControl), new PropertyMetadata(null));


    }
}
