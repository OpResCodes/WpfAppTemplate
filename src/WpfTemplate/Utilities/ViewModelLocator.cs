/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:WpfTemplate.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

#nullable enable
using System.ComponentModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System.Windows;
using WpfTemplate.Design;
using WpfTemplate.ViewModel;
using WpfTemplate.ViewModel.Base;

namespace WpfTemplate.Utilities
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    internal class ViewModelLocator : VmBase
    {

        public ViewModelLocator()
        {
            Navigate = new AutoRelayCommand<string>(ChangeView,
                (arg) => !IsBusy);
            Navigate.DependsOn(() => IsBusy);

            DesignData = new DesignData();

            _activeEditor = Get<VmMain>();//compiler null check
            ChangeView("main");
        }

        static ViewModelLocator()
        {
            GalaSoft.MvvmLight.Threading.DispatcherHelper.Initialize();
            Utilities.DependencyConfiguration.Configure(); //Dependency injection here
        }

        /// <summary>
        /// Returns the StaticResource instance
        /// </summary>
        public static ViewModelLocator? Instance => Application.Current.Resources["Locator"] as ViewModelLocator;

        public AutoRelayCommand<string> Navigate { get; }
        
        private void ChangeView(string target)
        {
            ActiveEditor = target switch
            {
                "main" => Get<VmMain>(),
                _ => Get<VmMain>(),
            };
        }

        public VmMain MainViewmodel => Get<VmMain>();

        public VmLogViewer LogViewmodel => Get<VmLogViewer>();

        public DesignData DesignData { get; }

        private T Get<T>()
        {
            return SimpleIoc.Default.GetInstance<T>();
        }

        private VmBase _activeEditor;
        public VmBase ActiveEditor
        {
            get => _activeEditor;
            set
            {
                if (value != _activeEditor && _activeEditor != null)
                {
                    _activeEditor.IsActive = false;
                    _activeEditor.PropertyChanged -= EditorPropertyChanged;
                }
#pragma warning disable CS8601 // Possible null reference assignment.
                Set(nameof(ActiveEditor), ref _activeEditor, value, true);
#pragma warning restore CS8601 // Possible null reference assignment.
                if (_activeEditor != null)
                {
                    _activeEditor.IsActive = true;
                    this.IsBusy = _activeEditor.IsBusy;
                    _activeEditor.PropertyChanged -= EditorPropertyChanged;
                    _activeEditor.PropertyChanged += EditorPropertyChanged;
                }
            }
        }

        private void EditorPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != null && e.PropertyName.Equals("IsBusy"))
            {
                this.IsBusy = _activeEditor.IsBusy;
            }
        }
    }
}