using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChangeTracking.Wpf;

/*
 * Below classes show the use of the UiWrapper<T> class to 
 * enhance a model class with UI notification updates for data binding
 * simple properties propagate change to the ui and to the model object
 * collection properties propagate collection changes to ui and model object
 * each wrapper exposes his inner model (i.e. to make database updates simple)
 * */


namespace WpfTemplate.Utilities.ExampleWrapping
{

    /// <summary>
    /// Example of a complex type to be used in a model class
    /// </summary>
    public class ExampleSubClass
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    /// <summary>
    /// Example model class to demonstrate typical Ui wrapping
    /// </summary>
    public class ExampleModel
    {
        public ExampleModel()
        {
            Label = "Test";
            SomeComplexProperty = new ExampleSubClass() { Id = 1, Name = "Test" };
            SomeCollection = new List<double>() { 1, 2, 3, 4 };
            SomeComplexCollection = new List<ExampleSubClass>();
        } 

        //simple property
        public string Label { get; set; }

        //complex property
        public ExampleSubClass SomeComplexProperty { get; set; }

        //collection
        public List<double> SomeCollection { get; set; }

        public List<ExampleSubClass> SomeComplexCollection { get; set; }
    }

    /// <summary>
    /// UI Wrapper to notify changes in the properties of the model object
    /// </summary>
    public class ExampleWrapper : UiModelWrapper<ExampleModel>
    {
        public ExampleWrapper(ExampleModel model) : base(model) { }
    
        public string Label
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string LabelOriginalValue { get { return GetOriginalValue<string>(nameof(Label)); } }

        public bool LabelIsChanged { get { return GetIsChanged(nameof(Label)); } }
        public ExampleSubclassWrapper SomeComplexProperty { get; private set; }

        public ObservableCollection<double> SomeCollection { get; private set; }

        public ChangeTrackingCollection<ExampleSubclassWrapper> SomeComplexCollection { get; private set; }

        //initialize ui wrappers for collections
        protected override void InitializeCollectionProperties()
        {
            if (Model.SomeCollection == null)
                throw new ArgumentNullException(nameof(Model.SomeCollection));
            if (Model.SomeComplexCollection == null)
                throw new ArgumentNullException(nameof(Model.SomeComplexCollection));

            //simple collection
            SomeCollection = new ObservableCollection<double>(Model.SomeCollection);
            //complex collection using LINQ
            SomeComplexCollection = new ChangeTrackingCollection<ExampleSubclassWrapper>(
                Model.SomeComplexCollection.Select(c => new ExampleSubclassWrapper(c))
                );
        }

        //initialize ui wrappers for complex types
        protected override void InitializeComplexProperties(ExampleModel model)
        {
            SomeComplexProperty = new ExampleSubclassWrapper(Model.SomeComplexProperty);
        }
    }

    /// <summary>
    /// UI Wrapper to notify changes in the properties of the model object
    /// </summary>
    public class ExampleSubclassWrapper : UiModelWrapper<ExampleSubClass>
    {
        public ExampleSubclassWrapper(ExampleSubClass model) : base(model) { }

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int Name
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

    }
    
}
