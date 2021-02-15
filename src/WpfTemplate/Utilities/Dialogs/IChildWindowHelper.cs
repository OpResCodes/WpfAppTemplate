namespace WpfTemplate.Dialogs
{
    public interface IChildWindowHelper : IDialogResultHelper
    {
        void WindowRequestsClose(object sender, RequestCloseEventArgs e);
    }
}
