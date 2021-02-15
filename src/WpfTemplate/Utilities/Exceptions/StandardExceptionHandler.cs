using GalaSoft.MvvmLight.Threading;
using System;
using GalaSoft.MvvmLight.Ioc;
using WpfTemplate.Dialogs;

namespace WpfTemplate.Utilities.ExceptionHandling
{
    public class StandardExceptionHandler : IExceptionHandler
    {
        public virtual void HandleException(Exception ex)
        {
            if (ex.IsFatal())
                throw ex; //application crash

            NLog.LogManager.GetCurrentClassLogger().Error(ex);
            var dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
            dialogService.ShowErrorPopup(ex.GetType().Name, $"{ex.Message}\n-------\n{ex.StackTrace}");
        }

        
    }
}

