using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfExtras
{ 

    [TemplateVisualState(GroupName = "CheckStates", Name = "Checked")]
    [TemplateVisualState(GroupName = "CheckStates", Name = "UnChecked")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Enabled")]
    public class VisualCheckBox : Control
    {
        static VisualCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VisualCheckBox), new FrameworkPropertyMetadata(typeof(VisualCheckBox)));

            FrameworkPropertyMetadata meta = new FrameworkPropertyMetadata();
            meta.DefaultValue = false;
            meta.AffectsMeasure = true;
            meta.PropertyChangedCallback = OnIsCheckedChanged;

            IsCheckedProperty = DependencyProperty.Register(
                "IsChecked",
                typeof(bool),
                typeof(VisualCheckBox), meta);

            IsCheckedContentProperty = DependencyProperty.Register(
                "IsCheckedContent",
                typeof(Visual),
                typeof(VisualCheckBox));
            IsCheckedContentProperty = DependencyProperty.Register(
                "IsUncheckedContent",
                typeof(Visual),
                typeof(VisualCheckBox));
        }

        private void ChangeVisualState(bool useTransitions = true)
        {
            if (IsChecked)
            {
                VisualStateManager.GoToState(this, "IsChecked", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "IsUnchecked", useTransitions);
            }
        }

        //wenn der visual tree erstellt wird
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.Template != null)
            {
                var grid = this.Template.FindName("PART_LayoutRoot", this) as Panel;
                if (grid != null)
                {
                    grid.MouseDown += (s, e) => { this.IsChecked = !this.IsChecked; };
                }
                ChangeVisualState(false);//keine transitions beim ersten setzen des visual states ausführen
            }
        }

        //callback für dependency property -> aktualisiere visual state
        static void OnIsCheckedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            VisualCheckBox vcb = o as VisualCheckBox;
            if (e.NewValue != e.OldValue)
            {
                vcb.ChangeVisualState(true);
            }
        }

        public static DependencyProperty IsCheckedProperty;
        public static DependencyProperty IsCheckedContentProperty;
        public static DependencyProperty IsUncheckedContentProperty;

        public Visual IsCheckedContent
        {
            get
            {
                return (Visual)GetValue(IsCheckedContentProperty);
            }
            set
            {
                SetValue(IsCheckedContentProperty, value);
            }
        }

        public Visual IsUncheckedContent
        {
            get
            {
                return (FrameworkElement)GetValue(IsUncheckedContentProperty);
            }
            set
            {
                SetValue(IsUncheckedContentProperty, value);
            }
        }

        public bool IsChecked
        {
            get
            {
                return ((bool)GetValue(IsCheckedProperty));
            }
            set
            {
                SetValue(IsCheckedProperty, value);
            }
        }

    }
}
