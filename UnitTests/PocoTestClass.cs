using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    class PocoTestClass
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public bool TestBool { get; set; }
        public List<PocoListItem> Items { get; set; }

        public PocoListItem ComplexProp { get; set; }
    }
}
