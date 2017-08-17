using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SaibaMvvmAdapter
{
    public abstract class ModelAdapter<T> : ViewModelBase, IRevertibleChangeTracking
    {
        private Dictionary<string, object> _changedProperties;
        private List<IRevertibleChangeTracking> _trackingObjects;
        public ModelAdapter(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            Model = model;
            _changedProperties = new Dictionary<string, object>();
            _trackingObjects = new List<IRevertibleChangeTracking>();
            InitializeComplexProperties(model);
            InitializeCollectionProperties(model);
        }

        protected virtual void InitializeComplexProperties(T model)
        {
        }

        protected virtual void InitializeCollectionProperties(T model)
        {
        }

        public T Model { get; }
        public bool IsChanged => _changedProperties.Count > 0 || _trackingObjects.Any(o => o.IsChanged);

        public void AcceptChanges()
        {
            _changedProperties.Clear();

            foreach (var complexObject in _trackingObjects)
            {
                complexObject.AcceptChanges();
            }

            OnPropertyChanged("");
        }

        public void RejectChanges()
        {
            foreach (var originalPropertyValue in _changedProperties)
            {
                typeof(T).GetRuntimeProperty(originalPropertyValue.Key).SetValue(Model, originalPropertyValue.Value);
            }

            _changedProperties.Clear();

            foreach (var complexObject in _trackingObjects)
            {
                complexObject.RejectChanges();
            }

            OnPropertyChanged("");
        }

        protected TValue GetValue<TValue>([CallerMemberName] string propertyName = null)
        {
            var propertyInfo = Model.GetType().GetRuntimeProperty(propertyName);
            return (TValue)propertyInfo.GetValue(Model);
        }

        protected TValue GetTrackedValue<TValue>(string propertyName)
        {
            return _changedProperties.ContainsKey(propertyName) ?
                (TValue)_changedProperties[propertyName] : GetValue<TValue>(propertyName);
        }

        protected bool IsPropertyChanged(string propertyName)
        {
            return _changedProperties.ContainsKey(propertyName);
        }

        protected void SetValue<TValue>(TValue value, [CallerMemberName] string propertyName = null)
        {
            //var propertyInfo = Model.GetType().GetTypeInfo().DeclaredProperties.Where(p => p.Name == propertyName).SingleOrDefault();   // GetProperty(propertyName);
            var propertyInfo = Model.GetType().GetRuntimeProperty(propertyName);
            var currentValue = propertyInfo.GetValue(Model);

            if (!Equals(currentValue, value))
            {
                TrackChangedProperty(currentValue, value, propertyName);

                propertyInfo.SetValue(Model, value);

                OnPropertyChanged(propertyName);
            }
        }

        private void TrackChangedProperty(object currentValue, object newValue, string propertyName)
        {
            if (!_changedProperties.ContainsKey(propertyName))
            {
                _changedProperties.Add(propertyName, currentValue);
                OnPropertyChanged(nameof(IsChanged));
            }
            else
            {
                if (Equals(_changedProperties[propertyName], newValue))
                {
                    _changedProperties.Remove(propertyName);
                    OnPropertyChanged(nameof(IsChanged));
                }
            }
        }

        protected void RegisterCollection<TAdapter, TModel>(ChangeTrackingCollection<TAdapter> adapterCollection, List<TModel> modelCollection)
            where TAdapter : ModelAdapter<TModel> where TModel : class
        {
            adapterCollection.CollectionChanged += (s, e) =>
            {
                modelCollection.Clear();
                modelCollection.AddRange(adapterCollection.Select(a => a.Model));
            };

            RegisterTrackingObject(adapterCollection);
        }


        protected void RegisterComplex<TModel>(ModelAdapter<TModel> adapter)
        {
            RegisterTrackingObject(adapter);
        }



        private void RegisterTrackingObject<TTrackingObject>(TTrackingObject trackingObject)
           where TTrackingObject : IRevertibleChangeTracking, INotifyPropertyChanged
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
                OnPropertyChanged(nameof(IsChanged));
            }
        }
    }
}
