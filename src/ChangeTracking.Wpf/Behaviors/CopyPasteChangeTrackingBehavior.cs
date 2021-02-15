//using WpfExtras.UiModelWrapping;
using System.Windows;

namespace WpfExtras.Behaviors
{

    /// <summary>
    /// This behavior handles pasting/pasted events to pause the change tracker of the 
    /// itemssource from updating until the pasting operation is complete
    /// This is used to speed up pasting lots of rows into a raddatagrid bound to
    /// a changetrackingcollection (it does not use AddRange() when pasting from
    /// clipboard!)
    /// </summary>
    //public static class CopyPasteChangeTrackingBehavior
    //{
    //    public static bool GetIsActive(DependencyObject obj)
    //    {
    //        return (bool)obj.GetValue(IsActiveProperty);
    //    }

    //    public static void SetIsActive(DependencyObject obj, bool value)
    //    {
    //        obj.SetValue(IsActiveProperty, value);
    //    }

    //    // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
    //    public static readonly DependencyProperty IsActiveProperty =
    //        DependencyProperty.RegisterAttached("IsActive", typeof(bool), 
    //            typeof(CopyPasteChangeTrackingBehavior), new PropertyMetadata(false,OnIsActivePropertyChanged));

    //    private static void OnIsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if(!(d is Telerik.Windows.Controls.RadGridView radGridView))
    //            return;
            
    //        if(((bool)e.NewValue))
    //        {
    //            radGridView.Pasting += RadGridView_Pasting;
    //            radGridView.Pasted += RadGridView_Pasted;
    //        }
    //        else
    //        {
    //            radGridView.Pasting -= RadGridView_Pasting;
    //            radGridView.Pasted -= RadGridView_Pasted;
    //        }

    //    }

    //    private static void RadGridView_Pasted(object sender, Telerik.Windows.RadRoutedEventArgs e)
    //    {
    //        if (!(sender is Telerik.Windows.Controls.RadGridView radGridView))
    //            return;

    //        if(radGridView.ItemsSource is IChangeTrackerRestarter changeTracker)
    //        {
    //            changeTracker.RestartChangeTracking();
    //        }
    //        //scroll to the last line
    //        radGridView.ScrollIndexIntoViewAsync(
    //            radGridView.Items.Count - 1, //the row 
    //            radGridView.Columns[0], //the column 
    //            new System.Action<FrameworkElement>((f) =>
    //            {
    //                (f as Telerik.Windows.Controls.GridView.GridViewRow).IsSelected = true; 
    //            }), 
    //            null);// the callback method; if it is not necessary, you may set that parameter to null; 
    //    }

    //    private static void RadGridView_Pasting(object sender, Telerik.Windows.Controls.GridViewClipboardEventArgs e)
    //    {
    //        if (!(sender is Telerik.Windows.Controls.RadGridView radGridView))
    //            return;

    //        if (radGridView.ItemsSource is IChangeTrackerRestarter changeTracker)
    //        {
    //            changeTracker.StopChangeTracking();
    //        }
    //    }
    //}
}
