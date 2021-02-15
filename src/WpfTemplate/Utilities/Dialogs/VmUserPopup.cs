
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;


namespace WpfTemplate.Dialogs
{
    public class VmUserPopup : IDialogResultHelper
    {
        public VmUserPopup(string popupText)
        {
            PopupText = popupText;
            ClosePopup = new RelayCommand(OnClose);
        }

        public string PopupText { get; set; }

        public ICommand ClosePopup { get; }

        private void OnClose() => CloseDialog(false);

        public void CloseDialog(bool withResult = false)
        {
            RequestCloseDialog?.Invoke(this, new RequestCloseEventArgs(withResult));
        }

        public event EventHandler<RequestCloseEventArgs> RequestCloseDialog;
    }
}
