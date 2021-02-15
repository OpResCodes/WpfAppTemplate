using System.Windows;
using System.Windows.Controls;

namespace WpfExtras
{
    [TemplateVisualState(GroupName = "StatusStates", Name = "IsDecreased")]
    [TemplateVisualState(GroupName = "StatusStates", Name = "IsIncreased")]
    [TemplateVisualState(GroupName = "StatusStates", Name = "IsUnchanged")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Enabled")]
    public class NumericUpdateVisualizer : Control
    {
        static NumericUpdateVisualizer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpdateVisualizer), new FrameworkPropertyMetadata(typeof(NumericUpdateVisualizer)));

            OriginalValueProperty = DependencyProperty.Register(
                "OriginalValue", 
                typeof(int), 
                typeof(NumericUpdateVisualizer), 
                new PropertyMetadata(0, OnOriginalValueChanged));

            CurrentValueProperty = DependencyProperty.Register(
                "CurrentValue", 
                typeof(int), 
                typeof(NumericUpdateVisualizer), 
                new PropertyMetadata(0, OnCurrentValueChanged));

            AutoUpdateOriginalValueProperty = DependencyProperty.Register(
                "AutoUpdateOriginalValue", 
                typeof(bool), 
                typeof(NumericUpdateVisualizer), 
                new PropertyMetadata(false));
        }

        public static readonly DependencyProperty OriginalValueProperty;

        public static readonly DependencyProperty CurrentValueProperty;

        public static readonly DependencyProperty AutoUpdateOriginalValueProperty;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Template != null)
            {
                ChangeVisualState(CurrentValue, OriginalValue);
            }
        }

        private void ChangeVisualState(int newValue, int oldValue)
        {
            if (newValue < oldValue)
            {
                VisualStateManager.GoToState(this, "IsDecreased", true);
            }
            else if (newValue > oldValue)
            {
                VisualStateManager.GoToState(this, "IsIncreased", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "IsUnchanged", true);
            }
        }

        private static void OnCurrentValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int nv = (int)e.NewValue;
            var currentControl = d as NumericUpdateVisualizer;
            currentControl.ChangeVisualState(nv, currentControl.OriginalValue);            
            if(currentControl.AutoUpdateOriginalValue)
            {
                currentControl.OriginalValue = (int)e.OldValue;
            }
        }

        private static void OnOriginalValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentControl = d as NumericUpdateVisualizer;
            //in case of auto updating, the visualisation has already been set by the other changehandler
            if (currentControl.AutoUpdateOriginalValue)
                return;
            var updatedOldValue = (int)e.NewValue;
            currentControl.ChangeVisualState(currentControl.CurrentValue, updatedOldValue);
        }

        public int CurrentValue
        {
            get { return (int)GetValue(CurrentValueProperty); }
            set { SetValue(CurrentValueProperty, value); }
        }

        public int OriginalValue
        {
            get { return (int)GetValue(OriginalValueProperty); }
            set { SetValue(OriginalValueProperty, value); }
        }

        public bool AutoUpdateOriginalValue
        {
            get { return (bool)GetValue(AutoUpdateOriginalValueProperty); }
            set { SetValue(AutoUpdateOriginalValueProperty, value); }
        }

    }
}
