using System;

namespace WpfTemplate.Dialogs
{
    public interface IDialogResultHelper
    {
        void CloseDialog(bool withResult = false);
        event EventHandler<RequestCloseEventArgs> RequestCloseDialog;
    }
}
