using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Input;

namespace WpfTemplate.Dialogs
{
    /// <summary>
    /// Use this Viewmodel to show an error Popup
    /// </summary>
    public class VmErrorPopup : IDialogResultHelper
    {
        public VmErrorPopup(string errorType, string errorText)
        {
            PopupText = errorText;
            ErrorType = errorType;
            ClosePopup = new RelayCommand(OnClose);
        }
        
        public string ErrorType { get; set; }
        
        public string PopupText { get; set; }

        private void OnClose() => CloseDialog();

        public ICommand ClosePopup { get; }

        public void CloseDialog(bool withResult = false)
        {
            RequestCloseDialog?.Invoke(this, new RequestCloseEventArgs(withResult));
        }

        public event EventHandler<RequestCloseEventArgs> RequestCloseDialog;
    }
}
