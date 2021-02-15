using System;

namespace WpfTemplate.Utilities.ExceptionHandling
{
    public class DebugExceptionHandler : IExceptionHandler
    {
        public virtual void HandleException(Exception ex)
        {
            if (ex.IsFatal())
                throw ex; //application crash
            System.Diagnostics.Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
        }
    }

}
