using System.Windows;
using System;
using WpfTemplate.Utilities.ExceptionHandling;
using GalaSoft.MvvmLight.Ioc;

namespace WpfTemplate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {

        }

        public App()
        {
            //default switch to english
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            SetupExceptionHandling();
            Exit += App_Exit;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            NLog.LogManager.Shutdown();
        }

        private void SetupExceptionHandling()
        {
            //global exception handling for ui thread
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            //global exception handling for unobserved exceptions of task scheduler (tasks without await and exception handling)
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            //exception handling for unhandled worker thread exceptions
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        /// <summary>
        /// Handles all unhandled Exceptions that occur in the UI Thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception.IsFatal())
            {
                return; //fatal exceptions will not be handled by standard handler 
            }
            this.HandleException(e.Exception);
            e.Handled = true;
        }

        /// <summary>
        /// Catch unhandled task exceptions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TaskScheduler_UnobservedTaskException(object sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
        {
            if (e.Exception.IsFatal()) { return; }

            var ae = e.Exception.Flatten();
            foreach (var ex in ae.InnerExceptions)
            {
                HandleException(ex);
            }
            //not to be given back to CLR
            e.SetObserved();
        }

        /// <summary>
        /// Catches unhandled exceptions in worker threads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;

            if (ex != null)
            {
                if (ex.IsFatal()) { return; }

                HandleException(ex);
            }
        }

        public void HandleException(Exception ex)
        {
            SimpleIoc.Default.GetInstance<IExceptionHandler>().HandleException(ex);
        }
        
    }

}
