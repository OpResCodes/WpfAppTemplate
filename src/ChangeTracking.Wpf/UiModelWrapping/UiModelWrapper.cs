using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ChangeTracking.Wpf
{
    public abstract class UiModelWrapper<T> : NotifyDataErrorInfoBase, IValidatableTrackingObject, IValidatableObject
    {

        private readonly Dictionary<string, object> _originalValues;
        //to track changes of child UiModelWrappers which should update this wrapper
        private readonly List<IValidatableTrackingObject> _trackingObjects; 

        public UiModelWrapper(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Model = model;
            _originalValues = new Dictionary<string, object>();
            _trackingObjects = new List<IValidatableTrackingObject>();
            InitializeComplexProperties(model);
            InitializeCollectionProperties();
            Validate();
        }

        protected virtual void InitializeCollectionProperties() { }

        protected virtual void InitializeComplexProperties(T model) { }

        [Display(AutoGenerateField = false)]
        public bool IsChanged
        {
            get
            {
                return (_originalValues.Count > 0 || _trackingObjects.Any(t => t.IsChanged));
            }
        }

        [Display(AutoGenerateField = false)]
        public bool IsValid
        {
            get
            {
                return !HasErrors && _trackingObjects.All(t => t.IsValid);
            }
        }

        public void AcceptChanges()
        {
            if (!IsChanged)
                return;

            _originalValues.Clear();
            foreach (var trackingObject in _trackingObjects)
            {
                trackingObject.AcceptChanges();
            }
            OnPropertyChanged(nameof(IsChanged));
            OnPropertyChanged(string.Empty);//all databindings will be reevaluated (refresh whole object in wpf ui)
        }

        public void RejectChanges()
        {
            if (!IsChanged)
                return;

            foreach (var originalValueEntry in _originalValues)
            {
                typeof(T).GetProperty(originalValueEntry.Key).SetValue(Model, originalValueEntry.Value);
            }
            _originalValues.Clear();
            foreach (var trackingObject in _trackingObjects)
            {
                trackingObject.RejectChanges();
            }
            Validate();
            OnPropertyChanged(nameof(IsChanged));
            OnPropertyChanged(string.Empty);//all databindings will update their values
        }

        [Display(AutoGenerateField = false)]
        public T Model { get; private set; }

        protected void RegisterCollectionUpdates<TWrapper, TModel>(
            ChangeTrackingCollection<TWrapper> wrapperCollection, ICollection<TModel> modelCollection)
            where TWrapper : UiModelWrapper<TModel>
        {
            wrapperCollection.CollectionChanged += (s, e) =>
            {
                modelCollection.Clear();
                foreach (var modelItem in wrapperCollection.Select(w => w.Model))
                {
                    modelCollection.Add(modelItem);
                }
                Validate();
            };
            RegisterTrackingObject(wrapperCollection);
        }

        protected void RegisterComplex<TModel>(UiModelWrapper<TModel> wrapper)
        {
            RegisterTrackingObject(wrapper);
        }

        protected TValue GetValue<TValue>([CallerMemberName] string propertyName = null)
        {
            var propertyInfo = Model.GetType().GetProperty(propertyName);
            return (TValue)propertyInfo.GetValue(Model);
        }

        protected TValue GetOriginalValue<TValue>(string propertyName)
        {
            return _originalValues.ContainsKey(propertyName) ? (TValue)_originalValues[propertyName] : GetValue<TValue>(propertyName);
        }

        protected bool GetIsChanged(string propertyName)
        {
            return _originalValues.ContainsKey(propertyName);
        }

        protected void SetValue<TValue>(TValue newValue, [CallerMemberName] string propertyName = null)
        {
            var propertyInfo = Model.GetType().GetProperty(propertyName);
            var currentValue = propertyInfo.GetValue(Model);
            if (!Equals(currentValue, newValue))
            {
                UpdateOriginalValue(currentValue, newValue, propertyName);
                propertyInfo.SetValue(Model, newValue);
                OnPropertyChanged(propertyName);
                OnPropertyChanged($"{propertyName}IsChanged");
                OnPropertyChanged(nameof(IsChanged));
                Validate();
            }
        }

        private void Validate()
        {
            bool wasValid = IsValid;
            ClearErrors();
            var results = new List<ValidationResult>();
            var context = new ValidationContext(this);
            Validator.TryValidateObject(this, context, results, true);
            results.AddRange(Validate(context));
            if (results.Any())
            {
                var propertyNames = results.SelectMany(r => r.MemberNames).Distinct().ToList();
                foreach (var propName in propertyNames)
                {
                    Errors[propName] = results.Where(r => r.MemberNames.Contains(propName))
                        .Select(r => r.ErrorMessage)
                        .Distinct()
                        .ToList();
                    OnErrorsChanged(propName);
                }
            }
            if (IsValid != wasValid)
            {
                OnPropertyChanged(nameof(IsValid));
            }
        }

        private void UpdateOriginalValue(object currentValue, object newValue, string propertyName)
        {
            if (!_originalValues.ContainsKey(propertyName))
            {
                _originalValues.Add(propertyName, currentValue);
                //RaisePropertyChanged(nameof(IsChanged));
            }
            else
            {
                if (Equals(_originalValues[propertyName], newValue))
                {
                    _originalValues.Remove(propertyName);
                    //RaisePropertyChanged(nameof(IsChanged));
                }
            }
        }

        private void RegisterTrackingObject(IValidatableTrackingObject trackingObject)
        {
            if (!_trackingObjects.Contains(trackingObject))
            {
                _trackingObjects.Add(trackingObject);
                trackingObject.PropertyChanged += TrackingObjectPropertyChanged;
            }
        }

        private void TrackingObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsChanged))
            {
                //broadcast
                OnPropertyChanged(nameof(IsChanged));
            }
            else if (e.PropertyName == nameof(IsValid))
            {
                //validate with changed tracking objects
                Validate();
            }
        }

        //class-wide validation (multiple properties validated together) -> implement in model-specific subclasses
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
