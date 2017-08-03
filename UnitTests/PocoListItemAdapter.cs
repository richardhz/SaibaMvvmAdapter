using SaibaMvvmAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    class PocoListItemAdapter : ModelAdapter<PocoListItem>
    {
        public PocoListItemAdapter(PocoListItem model) : base(model)
        {}

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string Title
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string Description
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string DescriptionOriginal => GetTrackedValue<string>(nameof(Description));
    }
}
