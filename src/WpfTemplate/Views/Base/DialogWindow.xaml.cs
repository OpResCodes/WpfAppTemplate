using System.Windows;

namespace WpfTemplate.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        public DialogWindow()
        {
            InitializeComponent();
            DialogPresenter.DataContextChanged += DialogPresenterDataContextChanged;
        }

        private void DialogPresenterDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var d = e.NewValue as IDialogResultHelper;

            if (d == null) return;

            d.RequestCloseDialog += d_RequestCloseDialog;
        }

        private void d_RequestCloseDialog(object sender, RequestCloseEventArgs e)
        {
            if (sender is IDialogResultHelper d)
            {
                d.RequestCloseDialog -= d_RequestCloseDialog;
            }
            DialogResult = e.DialogResult;
        }

    }
}
