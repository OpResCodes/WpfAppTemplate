using System;

namespace WpfTemplate.Utilities.ExceptionHandling
{
    public interface IExceptionHandler
    {
        void HandleException(Exception ex);
    }
}