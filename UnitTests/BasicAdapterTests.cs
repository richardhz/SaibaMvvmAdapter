using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class BasicAdapterTests
    {
        private PocoTestClass _tester;
        private PocoListItem _itemList;

        [TestInitialize]
        public void Initialize()
        {
            _itemList = new PocoListItem { Id = 2, Title = "TestItem02", Description = "Description02" };
            _tester = new PocoTestClass
            {
                TestId = 25,
                TestName = "Roger",
                TestBool = false,
                Items = new List<PocoListItem>
                {
                    new PocoListItem { Id = 1, Title = "TestItem01", Description = "Description01" },
                    _itemList
                },
                ComplexProp = new PocoListItem { Id = 2, Title = "Complex Property", Description = "Like address" }
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowArgumentNullExceptionIfModelIsNull()
        {
            var adapter = new PocoTestAdapter(null);
        }

        [TestMethod]
        public void Original_Model_Should_Be_In_Adapter_ModelProperty()
        {
            var adapter = new PocoTestAdapter(_tester);
            Assert.AreEqual(_tester, adapter.Model);
        }

        [TestMethod]
        public void AdapterProperty_Should_Be_Equal_To_Contained_ModelProperty()
        {
            var adapter = new PocoTestAdapter(_tester);
            Assert.AreEqual(_tester.TestName, adapter.TestName);
        }

        [TestMethod]
        public void Contained_ModelProperty_Should_Be_Set_To_AdapterProperty()
        {
            var adapter = new PocoTestAdapter(_tester);
            adapter.TestName = "Oranges";
            Assert.AreEqual("Oranges", _tester.TestName);
        }
    }
}
