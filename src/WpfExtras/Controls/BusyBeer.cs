using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WpfExtras
{

    [TemplateVisualState(Name = "DeterminateState", GroupName = "ProgressStates")]
    [TemplateVisualState(Name = "IndeterminateState", GroupName = "ProgressStates")]
    [TemplatePart(Name ="PART_BeerMaxFill",Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_BeerFill", Type = typeof(Rectangle))]
    public class BusyBeer : Control
    {
        static BusyBeer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyBeer), new FrameworkPropertyMetadata(typeof(BusyBeer)));
        }

        public BusyBeer()
        {

        }

        private FrameworkElement maxFill { get; set; }

        private FrameworkElement currentfill { get; set; }

        private void ChangeVisualState(bool useTransitions)
        {
            if (IsIndeterminate)
            {
                VisualStateManager.GoToState(this, "IndeterminateState", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "DeterminateState", useTransitions);
            }

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            maxFill = (FrameworkElement)this.Template.FindName("PART_BeerMaxFill",this);
            currentfill = (FrameworkElement)this.Template.FindName("PART_BeerFill", this);

            ChangeVisualState(false);
        }
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(BusyBeer), new PropertyMetadata(0d));


        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(BusyBeer), new PropertyMetadata(100d));


        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(BusyBeer), new PropertyMetadata(0d,
                new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var beer = d as BusyBeer;
            if (beer.IsIndeterminate)
                return;

            double value = (double)e.NewValue;
            double percentage = value / beer.Maximum;
            double updateHeight = beer.maxFill.ActualHeight * percentage;

            DoubleAnimation heightChangeAnimation = new DoubleAnimation();
            heightChangeAnimation.To = updateHeight;
            heightChangeAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(50));
            beer.currentfill.BeginAnimation(FrameworkElement.HeightProperty, heightChangeAnimation);
            
        }

        public bool IsIndeterminate
        {
            get { return (bool)GetValue(IsIndeterminateProperty); }
            set { SetValue(IsIndeterminateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsIndeterminate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register("IsIndeterminate", typeof(bool), 
                typeof(BusyBeer), 
                new PropertyMetadata(true, new PropertyChangedCallback(OnStateChanged)));


        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BusyBeer beer = d as BusyBeer;
            if(e.NewValue != e.OldValue)
            {
                beer.ChangeVisualState(true);            
            }
        }

    }
}
