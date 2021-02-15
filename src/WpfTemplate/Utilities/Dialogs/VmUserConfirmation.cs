using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Input;

namespace WpfTemplate.Dialogs
{
    public class VmUserConfirmation : IDialogResultHelper
    {

        public VmUserConfirmation(string title, string question)
        {
            UserConfirms = new RelayCommand(OnConfirm);
            UserDoesNotConfirm = new RelayCommand(OnUnconfirm);
            Title = title;
            Question = question;
        }

        public string Title { get; }

        public string Question { get; }

        public ICommand UserConfirms { get; }

        public ICommand UserDoesNotConfirm { get; }

        private void OnConfirm()
        {
            CloseDialog(true);
        }

        private void OnUnconfirm()
        {
            CloseDialog(false);
        }

        public void CloseDialog(bool withResult = false)
        {
            RequestCloseDialog?.Invoke(this, new RequestCloseEventArgs(withResult));
        }

        public event EventHandler<RequestCloseEventArgs> RequestCloseDialog;
    }
}
