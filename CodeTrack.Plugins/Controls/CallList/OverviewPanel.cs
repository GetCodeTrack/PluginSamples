using CodeTrack.Interface.Objects;
using CodeTrack.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CodeTrack.Plugins.Controls
{
    public class OverviewPanel : Panel
    {




        public IStoreViewDocument ViewDocument
        {
            get { return (IStoreViewDocument)GetValue(ViewDocumentProperty); }
            set { SetValue(ViewDocumentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewDocumentProperty =
            DependencyProperty.Register("ViewDocument", typeof(IStoreViewDocument), typeof(OverviewPanel), new PropertyMetadata(null, ReInit));




        public long CallPtr
        {
            get { return (long)GetValue(CallPtrProperty); }
            set { SetValue(CallPtrProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallPtrProperty =
            DependencyProperty.Register("CallPtr", typeof(long), typeof(OverviewPanel), new PropertyMetadata((long)0, ReInit));



        private static void ReInit(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var p = ((OverviewPanel)d);
            p.InitInternal();


        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InitInternal();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var rect = new Rect(0, 0, double.IsInfinity(availableSize.Width) ? 9999 : availableSize.Width, double.IsInfinity(availableSize.Height) ? 9999 : availableSize.Height);
            foreach (var child in Children)
            {
                
                ((UIElement)child).Measure(rect.Size);
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach(var child in Children)
            {
                ((UIElement)child).Arrange(new Rect(finalSize));
            }
            return base.ArrangeOverride(finalSize);

        }

        private void InitInternal()
        {
            try
            {
                if (ViewDocument == null || CallPtr == 0)
                {
                    if (Children.Count != 0)
                    {
                        Children.Clear();
                    }
                    return;
                }



                var ctrl = ViewDocument.BuildOverViewPanel(CallPtr);
                ctrl.Height = 120;

                Children.Add(ctrl);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
