using WpfTemplate.Utilities.ExceptionHandling;
using GalaSoft.MvvmLight.Ioc;
using NLog;
using NLog.Targets;
using System;
using NLog.Config;
using WpfTemplate.Dialogs;
using WpfTemplate.ViewModel;
using WpfTemplate.ViewModel.Base;

namespace WpfTemplate.Utilities
{
    public class DependencyConfiguration
    {

        internal static void Configure()
        {
            RegisterServices();
            RegisterViewModels();
        }

        private static void RegisterServices()
        {
            Register<IDialogService, WpfDialogService>(true);
            Register<IExceptionHandler, StandardExceptionHandler>(true);
            SimpleIoc.Default.Register<AppSettingsService>(() => AppSettingsService.Create("MyCoolApp"));
            ConfigureLoggingService();
        }

        private static void ConfigureLoggingService()
        {
            //Logging
            //public class MyClass
            //{
            //    private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            //}
            // logger.Info("Hello {0}", "Earth");
            // https://github.com/nlog/nlog/wiki/Tutorial

            LoggingConfiguration config = new NLog.Config.LoggingConfiguration();
            DebuggerTarget debugLogTarget = new NLog.Targets.DebuggerTarget("string");
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, debugLogTarget);


            //FileTarget logfileTarget = new NLog.Targets.FileTarget("logfile") { FileName = "log.txt" };
            //config.AddRule(LogLevel.Info, LogLevel.Fatal, logfileTarget);
            //should use appdatafolder from appsettingsservice for file-logging!

            NLog.LogManager.Configuration = config;
            LogManager.ReconfigExistingLoggers();
            LogManager.GetCurrentClassLogger().Info("Application started.");

            //note: the VmLogViewer viewmodel defines an additional in-memory logging target
            //which loggs into an observablecollection that is bound to a datagrid view

        }

        private static void RegisterViewModels()
        {
            Register<VmMain>();
            Register<VmLogViewer>(true);//to enable logging right away
        }

        #region Registering

        /// <summary>
        /// Save register in the dependency container. Checks if the type is already registered to avoid design time error messages.
        /// </summary>
        /// <typeparam name="TI">The interface that needs to be implemented</typeparam>
        /// <typeparam name="TC">the implementing type of the interface</typeparam>
        /// <param name="createImmidiately">true if the object is created instantly</param>
        private static void Register<TI, TC>(bool createImmidiately = false)
            where TI : class
            where TC : class, TI
        {
            if (!SimpleIoc.Default.IsRegistered<TI>())
            {
                SimpleIoc.Default.Register<TI, TC>(createImmidiately);
            }
        }

        /// <summary>
        /// Save register in the dependency container. Checks if the type is already registered to avoid design time error messages.
        /// </summary>
        /// <typeparam name="TC">The class that needs to be registered</typeparam>
        /// <param name="createImmidiately">true if the object is created instantly</param>
        private static void Register<TC>(bool createImmidiately = false) where TC : class
        {
            if (!SimpleIoc.Default.IsRegistered<TC>())
            {
                SimpleIoc.Default.Register<TC>(createImmidiately);
            }
        }

        #endregion

    }
}
