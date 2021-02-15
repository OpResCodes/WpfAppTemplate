using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace WpfTemplate.Utilities.ExceptionHandling
{
    public static class ExceptionHandlingExtension
    {
        /// <summary>
        /// Test if the Exception is a fatal exception that should not be handled by the application
        /// </summary>
        /// <param name="exception">The Exception that needs to be checked</param>
        /// <returns>True, if the Exception is a fatal Exception, else false</returns>
        public static bool IsFatal(this Exception exception)
        {
            //alle Ausnahmen und innere Ausnahmen durchgehen
            while (exception != null)
            {
                if ((exception is OutOfMemoryException && !(exception is InsufficientMemoryException)) ||
                    exception is ThreadAbortException ||
                    exception is SEHException ||
                    exception is StackOverflowException)
                {
                    //fatale Ausnahme
                    return true;
                }
                if (!(exception is TypeInitializationException) &&
                    !(exception is TargetInvocationException))
                {
                    /* 
                     * Diese können als innere Ausnahme eine fatale Ausnahme referenzieren
                     * daher Abbruch der Schleife möglich, falls keine Ausnahme des Typs
                    */
                    break;
                }
                //referenziere innere Ausnahme
                exception = exception.InnerException;
            }
            //keine fatale Ausnahme, wenn Code hier ankommt
            return false;
        }
    }
}
