using System;
using System.ComponentModel;
using System.Windows;

namespace WpfTemplate.Dialogs
{
    /// <summary>
    /// Interaction logic for ChildWindow.xaml
    /// </summary>
    public partial class ChildWindow : Window
    {
        private IChildWindowHelper viewmodel = null;
        private bool closeRequestByViewModel = false;

        public ChildWindow()
        {
            InitializeComponent();
            DialogPresenter.DataContextChanged += DialogPresenterDataContextChanged;
        }
        
        //set up events
        private void DialogPresenterDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            viewmodel = e.NewValue as IChildWindowHelper;
            if (viewmodel != null)
            {
                viewmodel.RequestCloseDialog += vm_RequestsClose;
                this.WindowRequestsClosing += viewmodel.WindowRequestsClose;
            }
        }

        private void vm_RequestsClose(object sender, RequestCloseEventArgs e)
        {
            if (viewmodel != null && !closeRequestByViewModel)
            {
                closeRequestByViewModel = true;
                Close();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!closeRequestByViewModel && viewmodel != null)
            {
                //close requested by clicking the close window X (->viewmodel should react)
                viewmodel.RequestCloseDialog -= vm_RequestsClose;//viewmodel might execute event when being notified of window close request ->prevent loop
                OnWindowRequestsClosing();
                //disconnict event
                WindowRequestsClosing -= viewmodel.WindowRequestsClose;
                viewmodel = null;
            }
            base.OnClosing(e);
        }
        
        private void OnWindowRequestsClosing()
        {
            var handler = WindowRequestsClosing;
            if (handler != null)
            {
                handler(this, new RequestCloseEventArgs(false));
            }
        }

        public event EventHandler<RequestCloseEventArgs> WindowRequestsClosing;

    }
}
