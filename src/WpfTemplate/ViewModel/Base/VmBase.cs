using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Reflection;
using System.Threading.Tasks;
using WpfTemplate.Dialogs;
using WpfTemplate.Utilities;
using WpfTemplate.Utilities.ExceptionHandling;

namespace WpfTemplate.ViewModel
{
    public abstract class VmBase : ViewModelBase, IExceptionHandler
    {

        protected bool _isActive = false;
        protected bool _isBusy = false;

        /// <summary>
        /// Indicates if the Viewmodel is busy
        /// </summary>
        public virtual bool IsBusy
        {
            get { return _isBusy; }
            set { Set(() => IsBusy, ref _isBusy, value, true); }
        }

        /// <summary>
        /// Indicates if the viewmodel is active and visible on screen
        /// </summary>
        public virtual bool IsActive
        {
            get { return _isActive; }
            set { Set(() => IsActive, ref _isActive, value, true); }
        }

        private bool _hideProgressWhenBusy = true;
        public bool HideProgressWhenBusy
        {
            get { return _hideProgressWhenBusy; }
            set { Set(() => HideProgressWhenBusy, ref _hideProgressWhenBusy, value); }
        }

        private double _currentProcessProgress = 0;
        public double CurrentProcessProgress
        {
            get { return _currentProcessProgress; }
            set { Set(() => CurrentProcessProgress, ref _currentProcessProgress, value); }
        }

        /// <summary>
        /// Get a service from the DI container
        /// </summary>
        /// <typeparam name="T">DI registered type</typeparam>
        /// <returns></returns>
        protected T GetInstance<T>()
        {
            return SimpleIoc.Default.GetInstance<T>();
        }

        protected T GetInstance<T>(string key)
        {
            return SimpleIoc.Default.GetInstance<T>(key);
        }

        /// <summary>
        /// Get the instance of the dialog service
        /// </summary>
        protected IDialogService DialogService => GetInstance<IDialogService>();

        /// <summary>
        /// Will also cleanup any autorelaycommand links
        /// </summary>
        public override void Cleanup()
        {
            //unregister AutoRelayCommands
            PropertyInfo[] properties = GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (typeof(AutoRelayCommand).IsAssignableFrom(property.PropertyType))
                {
                    MethodInfo getterMethodInfo = property.GetGetMethod();
                    var command = getterMethodInfo.Invoke(this, null) as AutoRelayCommand;
                    command.CleanUp();
                }
            }
            //unregister Messenger
            base.Cleanup();
        }

        /// <summary>
        /// Handle the exceptions with an IExceptionHandler
        /// </summary>
        /// <param name="ex"></param>
        public void HandleException(Exception ex)
        {
            GetInstance<IExceptionHandler>().HandleException(ex);
        }

        protected async Task DoSomething(Func<Task> action, bool withProgress = false)
        {
            bool hide = _hideProgressWhenBusy;
            try
            {
                HideProgressWhenBusy = !withProgress;
                CurrentProcessProgress = 0;
                IsBusy = true;
                await action();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                IsBusy = false;
                HideProgressWhenBusy = hide;
            }
        }

        public static new bool IsInDesignModeStatic => System.ComponentModel.
            DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject());

        protected new bool IsInDesignMode => VmBase.IsInDesignModeStatic;
    }
}
