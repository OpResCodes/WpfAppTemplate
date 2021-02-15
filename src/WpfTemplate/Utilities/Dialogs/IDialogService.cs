namespace WpfTemplate.Dialogs
{
    public interface IDialogService
    {

        /// <summary>
        /// shows a custom user dialog
        /// </summary>
        /// <param name="title"></param>
        /// <param name="datacontext"></param>
        /// <returns></returns>
        bool? ShowCustomUserDialog(string title, object datacontext, bool fixedWindow = true);

        /// <summary>
        /// Display a child window (non modal)
        /// </summary>
        /// <param name="title">Title of the Child Window</param>
        /// <param name="datacontext">DataContext of the Child</param>
        void ShowCustomChildWindow(string title, object datacontext);

        /// <summary>
        /// Shows a yes/no/abort Messagebox
        /// </summary>
        /// <param name="confirmationMsg"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        bool ShowConfirmationDialog(string confirmationMsg, string title);

        /// <summary>
        /// Shows a standard Messagebox
        /// </summary>
        /// <param name="title"></param>
        /// <param name="popupMessage"></param>
        void ShowUserPopup(string title, string popupMessage);

        /// <summary>
        /// Display indication of error
        /// </summary>
        /// <param name="title">The Popup Title</param>
        /// <param name="popupMessage">The Error Message</param>
        void ShowErrorPopup(string title, string popupMessage);

        /// <summary>
        /// Opens a File Open Dialog and returns true if a filename was set to the out parameter
        /// </summary>
        /// <param name="defaultDirectory"></param>
        /// <param name="defaultExtension"></param>
        /// <param name="ResultFileName"></param>
        /// <returns></returns>
        bool TryOpenFileDialog(string defaultDirectory, string defaultExtension, out string ResultFileName);

        /// <summary>
        /// Opens a File Save Dialog and returns true if a filename was set to the out parameter
        /// </summary>
        /// <param name="defaultDirectory"></param>
        /// <param name="defaultName"></param>
        /// <param name="defaultExtension"></param>
        /// <param name="ResultFileName"></param>
        /// <returns></returns>
        bool TrySaveFileDialog(string defaultDirectory, string defaultName, string defaultExtension, out string ResultFileName);

        /// <summary>
        /// Trys to select a folder from a folder browser dialog
        /// </summary>
        /// <param name="ResultFileName"></param>
        /// <returns></returns>
        bool TrySelectFolderDialog(out string ResultFileName);

        ProgressHandling ShowProgressPopup(string popupTitle, string headline);


    }
}
