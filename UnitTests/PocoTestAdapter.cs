using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaibaMvvmAdapter;
using System.Collections.ObjectModel;

namespace UnitTests
{
    class PocoTestAdapter : ModelAdapter<PocoTestClass>
    {
        public PocoTestAdapter(PocoTestClass model ) : base(model)
        {
            InitializeCollections(model);
            InitializeComplexProperties(model);
        }

        private void InitializeCollections(PocoTestClass model)
        {
            //The creation of adapter list items is not tested here, this would be a user test and not a library test.
            Items = new ChangeTrackingCollection<PocoListItemAdapter>(model.Items.Select(i => new PocoListItemAdapter(i))); 
            //However, if used the RegisterCollection method should do what it says on the tin. 
            RegisterCollection(Items, model.Items);
        }

        private void InitializeComplexProperties(PocoTestClass model)
        {
            ComplexProp = new PocoListItemAdapter(model.ComplexProp);
            RegisterComplex(ComplexProp);
        }

        public int TestId
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public string TestName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string TestNameOriginal => GetTrackedValue<string>(nameof(TestName));

        public bool TestIdHasChanged => IsPropertyChanged(nameof(TestId));

        public bool TestBool
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public ChangeTrackingCollection<PocoListItemAdapter> Items { get; private set; }

        public PocoListItemAdapter ComplexProp { get; private set; }
       
    }
}
