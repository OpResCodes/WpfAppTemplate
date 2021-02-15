using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
//using Telerik.Windows.Controls;
//using Telerik.Windows.Controls.GridView;

namespace WpfExtras.Behaviors
{
    /// <summary>
    /// This behavior provides attached properties to a control
    /// if the behavior is set to IsActive = true it will automatically create data bindings for 
    /// 2 additional attached properties IsChanged and OriginalValue
    /// These attached properties can be used in a style to trigger the change of appearance 
    /// and to display the original value somewhere (in a tooltip for example)
    /// it also provides the possibility to attach a converter to the original value property
    /// in the template the attached behavior is set by using a custom base style that the controls styles base on
    /// </summary>
    public static class ChangeBehavior
    {
        public static readonly DependencyProperty IsChangedProperty;
        public static readonly DependencyProperty OriginalValueProperty;
        public static readonly DependencyProperty IsActiveProperty;
        public static readonly DependencyProperty OriginalValueConverterProperty;
        private static readonly Dictionary<Type, DependencyProperty> _defaultProperties;


        static ChangeBehavior()
        {
            IsActiveProperty = DependencyProperty.RegisterAttached("IsActive", typeof(bool), typeof(ChangeBehavior), new PropertyMetadata(false, OnIsActiveChanged));
            IsChangedProperty = DependencyProperty.RegisterAttached("IsChanged", typeof(bool), typeof(ChangeBehavior), new PropertyMetadata(false));
            OriginalValueProperty = DependencyProperty.RegisterAttached("OriginalValue", typeof(object), typeof(ChangeBehavior), new PropertyMetadata(null));
            OriginalValueConverterProperty = DependencyProperty.RegisterAttached("OriginalValueConverter", typeof(IValueConverter), typeof(ChangeBehavior), new PropertyMetadata(null, OnOriginalValueConverterPropertyChanged));

            _defaultProperties = new Dictionary<Type, DependencyProperty>()
            {
                [typeof(TextBox)] = TextBox.TextProperty,
                [typeof(CheckBox)] = ToggleButton.IsCheckedProperty,
                [typeof(DatePicker)] = DatePicker.SelectedDateProperty,
                [typeof(ComboBox)] = Selector.SelectedValueProperty,
                //[typeof(RadComboBox)] = RadComboBox.SelectedValueProperty,
                //[typeof(RadToggleButton)] = RadToggleButton.IsCheckedProperty,
                [typeof(TextBlock)] = TextBlock.TextProperty
            };
        }

        public static IValueConverter GetOriginalValueConverter(DependencyObject obj)
        {
            return (IValueConverter)obj.GetValue(OriginalValueConverterProperty);
        }

        public static void SetOriginalValueConverter(DependencyObject obj, IValueConverter value)
        {
            obj.SetValue(OriginalValueConverterProperty, value);
        }

        public static bool GetIsActive(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsActiveProperty);
        }

        public static void SetIsActive(DependencyObject obj, bool value)
        {
            obj.SetValue(IsActiveProperty, value);
        }

        public static bool GetIsChanged(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsChangedProperty);
        }

        public static void SetIsChanged(DependencyObject obj, bool value)
        {
            obj.SetValue(IsChangedProperty, value);
        }

        public static object GetOriginalValue(DependencyObject obj)
        {
            return (object)obj.GetValue(OriginalValueProperty);
        }

        public static void SetOriginalValue(DependencyObject obj, object value)
        {
            obj.SetValue(OriginalValueProperty, value);
        }

        /// <summary>
        /// If the Behavior is active, we create 2 additional databindings to bind the IsChanged and OriginalValue Properties
        /// that correspond to the property that is databound to the control (i.e. a string property of the viewmodel bound to 
        /// the text property of a textbox
        /// the control's style can then use triggers to change appearance based on the IsChanged
        /// and it can use the OriginalValue in a popup/tooltip etc
        /// </summary>
        /// <param name="d">The Dependency Object (i.e. Control) that the behavior is placed upon</param>
        /// <param name="e">DependencyPropertyChangedEventArgs</param>
        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement ctrl))
            {
                return;
            }

            if (!ctrl.IsInitialized)
            {
                ctrl.Initialized += TargetControl_Initialized;
            }
            else
            {
                //d is the object where the isactive property was set (i.e. a textbox, a combobox)
                //mapping to the bound property, i.e. from textbox -> text property
                if (_defaultProperties.ContainsKey(d.GetType()))
                {
                    //get the control property that is databound
                    var defaultProperty = _defaultProperties[d.GetType()];
                    if ((bool)e.NewValue) //IsActive = True
                    {
                        //get databinding of the control property
                        var binding = BindingOperations.GetBinding(d, defaultProperty);
                        if (binding != null)
                        {
                            //get the binding path (Property on the viewmodel)
                            string bindingPath = binding.Path.Path;
                            //create databinding for the corresponding IsChanged property of the viewmodel
                            BindingOperations.SetBinding(d, IsChangedProperty, new Binding(bindingPath + "IsChanged"));
                            //create databinding for the corresponding OriginalValue property of the viewmodel (including a possible converter)
                            CreateOriginalValueBinding(d, bindingPath + "OriginalValue");
                        }
                    }
                    else //IsActive = false
                    {
                        //clear the additional two bindings
                        BindingOperations.ClearBinding(d, IsChangedProperty);
                        BindingOperations.ClearBinding(d, OriginalValueProperty);
                    }
                }
            }


        }

        private static void TargetControl_Initialized(object sender, EventArgs e)
        {
            var d = (DependencyObject)sender;
            if (GetIsActive(d))
            {
                if (_defaultProperties.ContainsKey(d.GetType()))
                {
                    //get the control property that is databound
                    var defaultProperty = _defaultProperties[d.GetType()];
                    //get databinding of the control property
                    var binding = BindingOperations.GetBinding(d, defaultProperty);
                    if (binding != null)
                    {
                        //get the binding path (property on the viewmodel)
                        string bindingPath = binding.Path.Path;
                        //create databinding for the corresponding IsChanged property of the viewmodel
                        BindingOperations.SetBinding(d, IsChangedProperty, new Binding(bindingPath + "IsChanged"));
                        //create databinding for the corresponding OriginalValue property of the viewmodel (including a possible converter)
                        CreateOriginalValueBinding(d, bindingPath + "OriginalValue");
                    }
                }
            }
            if (d is Control control)
            {
                control.Initialized -= TargetControl_Initialized;
            }
        }

        //if a value converter for the original value of the change tracked viewmodel is specified, update the binding 
        //to the originalvalue with the converter
        private static void OnOriginalValueConverterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (BindingOperations.GetBinding(d, OriginalValueProperty) is Binding originalValueBinding)
            {
                CreateOriginalValueBinding(d, originalValueBinding.Path.Path);
            }
        }

        private static void CreateOriginalValueBinding(DependencyObject d, string path)
        {
            Binding newBinding = new Binding(path)
            {
                Converter = GetOriginalValueConverter(d),
                ConverterParameter = d
            };
            BindingOperations.SetBinding(d, OriginalValueProperty, newBinding);
        }
    }
}
