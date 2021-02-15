using System;
using System.Windows;
using Telerik.Windows.Controls;

/* ViewModel is the DataContext of the Dialog View and must implement ICustomDialogResultHelper
 *   
 * EXAMPLE Viewmodel:  
 * 
 *  public void CloseDialog(bool withResult = false)
 *  {
 *     RequestCloseDialog?.Invoke(this, new RequestCloseEventArgs(withResult));
 *  }
 *  public event EventHandler<RequestCloseEventArgs> RequestCloseDialog;
 * 
 * ---------
 * To call the Dialog use implemented IDialogService:
 * 
 *  var xyz = myViewModelForTheDialog();
 *  
 *  bool? DialogResult = DialogService.ShowCustomUserDialog("Title of dialog window",xyz);
 *  //grab results:
 *  if (dialogresult.HasValue && dialogresult.Value==true)
 *  {
 *      do stuff with xyz (get values from state)
 *  }
 * 
 * */

namespace WpfTemplate.Dialogs
{
    public class WpfDialogService : IDialogService
    {

        /// <summary>
        /// Opens a File Open Dialog and returns true if a filename was set to the out parameter
        /// </summary>
        /// <param name="defaultDirectory"></param>
        /// <param name="defaultExtension"></param>
        /// <param name="ResultFileName"></param>
        /// <returns></returns>
        public bool TryOpenFileDialog(string defaultDirectory, string defaultExtension, out string ResultFileName)
        {
            // Create OpenFileDialog
            RadOpenFileDialog dlg = new RadOpenFileDialog();
            dlg.Filter = $"Files ({defaultExtension})|*{defaultExtension}";
            dlg.DefaultExt = defaultExtension;
            if (!string.IsNullOrWhiteSpace(defaultDirectory)) { dlg.InitialDirectory = defaultDirectory; }
            else
            {
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            bool? result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                ResultFileName = dlg.FileName;
                return true;
            }
            ResultFileName = null;
            return false;
        }

        /// <summary>
        /// Opens a File Save Dialog and returns true if a filename was set to the out parameter
        /// </summary>
        /// <param name="defaultDirectory"></param>
        /// <param name="defaultName"></param>
        /// <param name="defaultExtension"></param>
        /// <param name="ResultFileName"></param>
        /// <returns></returns>
        public bool TrySaveFileDialog(string defaultDirectory, string defaultName, string defaultExtension, out string ResultFileName)
        {
            RadSaveFileDialog saveFileDialog = new RadSaveFileDialog();
            if (!string.IsNullOrEmpty(defaultDirectory)) { saveFileDialog.InitialDirectory = defaultDirectory; }
            else
            {
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            saveFileDialog.FileName = defaultName;
            saveFileDialog.DefaultExt = defaultExtension;
            saveFileDialog.Filter = $"Files ({defaultExtension})|*{defaultExtension}";
            bool? result = saveFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                ResultFileName = saveFileDialog.FileName;
                return true;
            }
            ResultFileName = null;
            return false;

        }

        /// <summary>
        /// Trys to select a folder from a folder browser dialog
        /// </summary>
        /// <param name="ResultFileName"></param>
        /// <returns></returns>
        public bool TrySelectFolderDialog(out string ResultFileName)
        {
            RadOpenFolderDialog folderDialog = new RadOpenFolderDialog();
            folderDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            bool? selected = folderDialog.ShowDialog();
            if (selected.HasValue && selected.Value)
            {
                ResultFileName = folderDialog.FileName;
                return true;
            }
            ResultFileName = string.Empty;
            return false;
        }

        /// <summary>
        /// shows a custom user dialog
        /// </summary>
        /// <param name="title"></param>
        /// <param name="datacontext"></param>
        /// <returns></returns>
        public bool? ShowCustomUserDialog(string title, object datacontext, bool fixedWindow = true)
        {
            var dialogWindow = new DialogWindow();
            dialogWindow.Title = title;
            dialogWindow.DialogPresenter.Content = datacontext; //view for the datacontext defined in Vm2View.xaml

            if (!fixedWindow)
            {
                dialogWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                dialogWindow.BorderThickness = new Thickness(0);
            }
            else
            {
                dialogWindow.Owner = Application.Current.MainWindow;
                dialogWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            return dialogWindow.ShowDialog();
        }

        public void ShowCustomChildWindow(string title, object datacontext)
        {
            ChildWindow win = new ChildWindow()
            {
                Title = title
            };
            win.DialogPresenter.Content = datacontext;
            win.Owner = Application.Current.MainWindow;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Show();
        }

        /// <summary>
        /// Shows a yes/no/abort Messagebox
        /// </summary>
        /// <param name="confirmationMsg"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool ShowConfirmationDialog(string confirmationMsg, string title)
        {
            var vm = new VmUserConfirmation(title, confirmationMsg);
            var result = ShowCustomUserDialog(title, vm, true);
            return (result != null && result.Value);
        }

        /// <summary>
        /// Shows a standard Messagebox
        /// </summary>
        /// <param name="title"></param>
        /// <param name="popupMessage"></param>
        public void ShowUserPopup(string title, string popupMessage)
        {
            var vm = new VmUserPopup(popupMessage);
            ShowCustomUserDialog(title, vm);
        }

        /// <summary>
        /// Display indication of error
        /// </summary>
        /// <param name="title">The Popup Title</param>
        /// <param name="popupMessage">The Error Message</param>
        public void ShowErrorPopup(string title, string popupMessage)
        {
            var vm = new VmErrorPopup(title, popupMessage);
            ShowCustomUserDialog(title, vm);
        }

        public ProgressHandling ShowProgressPopup(string popupTitle, string headline)
        {
            ProgressHandling handling = new ProgressHandling();
            VmProgressReport progVm = new VmProgressReport(handling.ProgressReport, handling.CancellationTokenSource)
            {
                HeadlineText = headline
            };
            ShowCustomChildWindow(popupTitle, progVm);
            return handling;
        }

    }
}
