using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class ChangeTrackingComplexPropertyTests
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
        public void When_Complex_Property_Is_Changed_The_Parent_IsChanged_Should_Be_Set()
        {
            var adapter = new PocoTestAdapter(_tester);
            adapter.ComplexProp.Description = "Something Different";
            Assert.IsTrue(adapter.IsChanged);

            adapter.ComplexProp.Description = "Like address";
            Assert.IsFalse(adapter.IsChanged);
        }

        [TestMethod]
        public void When_Complex_Property_Is_Changed_Should_Raise_PropertyChangedEvent_For_Parent()
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
            adapter.ComplexProp.Description = "Something Different";
            Assert.AreEqual(true, isRaised);
        }

        [TestMethod]
        public void Should_Accept_Changes_For_Complex_Properties()
        {
            var adapter = new PocoTestAdapter(_tester);

            adapter.ComplexProp.Description = "Something Different";
            Assert.IsTrue(adapter.ComplexProp.IsChanged);
            Assert.IsTrue(adapter.IsChanged);

            adapter.AcceptChanges();

            Assert.AreEqual("Something Different", adapter.ComplexProp.Description);
            Assert.IsFalse(adapter.ComplexProp.IsChanged);
            Assert.IsFalse(adapter.IsChanged);
        }

        [TestMethod]
        public void Should_Reject_Changes_For_Complex_Properties()
        {
            var adapter = new PocoTestAdapter(_tester);

            adapter.ComplexProp.Description = "Something Different";
            Assert.AreEqual("Something Different", adapter.ComplexProp.Description);
            Assert.IsTrue(adapter.ComplexProp.IsChanged);
            Assert.IsTrue(adapter.IsChanged);

            adapter.RejectChanges();

            Assert.AreEqual("Like address", adapter.ComplexProp.Description);
            Assert.IsFalse(adapter.ComplexProp.IsChanged);
            Assert.IsFalse(adapter.IsChanged);

        }
    }
}
