using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class ChangeTrackingTests
    {
        private PocoTestClass _tester;
        private PocoListItem _listItem;

        [TestInitialize]
        public void Initialize()
        {
            _listItem = new PocoListItem { Id = 2, Title = "TestItem02", Description = "Description02" };
            _tester = new PocoTestClass
            {
                TestId = 25,
                TestName = "Roger",
                TestBool = false,
                Items = new List<PocoListItem>
                {
                    new PocoListItem { Id = 1, Title = "TestItem01", Description = "Description01" },
                    _listItem
                },
                ComplexProp = new PocoListItem { Id = 2, Title = "Complex Property", Description = "Like address" }
            };
        }

        [TestMethod]
        public void Original_Value_Is_Stored()
        {
            var adapter = new PocoTestAdapter(_tester);
            Assert.AreEqual("Roger", adapter.TestNameOriginal);
            adapter.TestName = "Boris";
            Assert.AreEqual("Roger", adapter.TestNameOriginal);

        }

        [TestMethod]
        public void HasChanged_Property_Is_Set_When_Property_Changes()
        {
            var adapter = new PocoTestAdapter(_tester);
            Assert.IsFalse(adapter.TestIdHasChanged);

            adapter.TestId = 100;
            Assert.IsTrue(adapter.TestIdHasChanged);
        }

        [TestMethod]
        public void HasChanged_Property_Is_False_When_Property_Changes_To_Original_Value()
        {
            var adapter = new PocoTestAdapter(_tester);
            Assert.IsFalse(adapter.TestIdHasChanged);

            adapter.TestId = 100;
            Assert.IsTrue(adapter.TestIdHasChanged);

            adapter.TestId = 25;
            Assert.IsFalse(adapter.TestIdHasChanged);
        }

        [TestMethod]
        public void Adapter_IsChanged_Property_Should_Be_Set_When_Any_Property_Is_Changed()
        {
            var adapter = new PocoTestAdapter(_tester);
            Assert.IsFalse(adapter.TestIdHasChanged);
            Assert.IsFalse(adapter.IsChanged);

            adapter.TestId = 100;
            Assert.IsTrue(adapter.TestIdHasChanged);
            Assert.IsTrue(adapter.IsChanged);

            adapter.TestId = 25;
            Assert.IsFalse(adapter.TestIdHasChanged);
            Assert.IsFalse(adapter.IsChanged);

            adapter.TestName = "Boris";
            Assert.IsTrue(adapter.IsChanged);
        }

        [TestMethod]
        public void Adapter_IsChanged_Property_Is_False_When_Properties_Change_To_Original_Values()
        {
            Initialize();
            var adapter = new PocoTestAdapter(_tester);
            Assert.IsFalse(adapter.IsChanged);

            adapter.TestId = 100;
            Assert.IsTrue(adapter.IsChanged);

            adapter.TestBool = true;
            adapter.TestId = 25;
            Assert.IsTrue(adapter.IsChanged);

            adapter.TestBool = false;
            Assert.IsFalse(adapter.IsChanged);
        }

        [TestMethod]
        public void Should_Raise_PropertyChangedEvent_For_Adapter_When_Any_Property_Is_Changed()
        {
            var isRaised = false;

            var adapter = new PocoTestAdapter(_tester);

            adapter.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(adapter.IsChanged))
                {
                    isRaised = true;
                }
            };
            adapter.TestId = 50;
            Assert.AreEqual(true, isRaised);
        }

        [TestMethod]
        public void Should_Accept_Changes()
        {
            var adapter = new PocoTestAdapter(_tester);
            adapter.TestName = "Sandra";
            adapter.TestId = 100;
            Assert.AreEqual("Sandra", adapter.TestName);
            Assert.AreEqual("Roger", adapter.TestNameOriginal);
            Assert.IsTrue(adapter.TestIdHasChanged);
            Assert.IsTrue(adapter.IsChanged);

            adapter.AcceptChanges();

            Assert.AreEqual("Sandra", adapter.TestName);
            Assert.AreEqual("Sandra", adapter.TestNameOriginal);
            Assert.IsFalse(adapter.TestIdHasChanged);
            Assert.IsFalse(adapter.IsChanged);

        }

        [TestMethod]
        public void Should_Reject_Changes()
        {
            var adapter = new PocoTestAdapter(_tester);
            adapter.TestName = "Sandra";
            adapter.TestId = 100;
            Assert.AreEqual("Sandra", adapter.TestName);
            Assert.AreEqual("Roger", adapter.TestNameOriginal);
            Assert.IsTrue(adapter.TestIdHasChanged);
            Assert.IsTrue(adapter.IsChanged);

            adapter.RejectChanges();

            Assert.AreEqual("Roger", adapter.TestName);
            Assert.AreEqual("Roger", adapter.TestNameOriginal);
            Assert.AreEqual(25, adapter.TestId);
            Assert.IsFalse(adapter.TestIdHasChanged);
            Assert.IsFalse(adapter.IsChanged);

        }
    }
}
