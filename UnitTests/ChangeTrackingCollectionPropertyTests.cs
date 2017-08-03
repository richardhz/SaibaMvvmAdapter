using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class ChangeTrackingCollectionPropertyTests
    {

        private PocoTestClass _tester;
        private PocoListItem _listItem;

        [TestInitialize]
        public void Initialize()
        {
            _tester = new PocoTestClass
            {
                TestId = 25,
                TestName = "Roger",
                TestBool = false,
                Items = new List<PocoListItem>
                {
                    new PocoListItem { Id = 1, Title = "TestItem01", Description = "Description01" },
                    new PocoListItem { Id = 2, Title = "TestItem02", Description = "Description02" }
                },
                ComplexProp = new PocoListItem { Id = 2, Title = "Complex Property", Description = "Like address" }

            };
        }

        [TestMethod]
        public void Should_Set_IsChanged_Property_On_Adapter()
        {
            var adapter = new PocoTestAdapter(_tester);
            var listItemToModify = adapter.Items.First();
            listItemToModify.Description = "modified item";

            Assert.IsTrue(adapter.IsChanged);

            listItemToModify.Description = "Description01";
            Assert.IsFalse(adapter.IsChanged);
        }

        [TestMethod]
        public void Should_Raise_PropertyChangedEvent_For_IsChanged_Property_On_Adapter()
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

            var listItemToModify = adapter.Items.First();
            listItemToModify.Description = "modified item";

            Assert.IsTrue(isRaised);
        }

        [TestMethod]
        public void Should_Accept_Changes_When_Called_On_Adapter()
        {
            var adapter = new PocoTestAdapter(_tester);

            var listItemToModify = adapter.Items.First();
            listItemToModify.Description = "modified item";

            Assert.IsTrue(adapter.IsChanged);

            adapter.AcceptChanges();

            Assert.IsFalse(adapter.IsChanged);
            Assert.AreEqual("modified item", listItemToModify.Description);
            Assert.AreEqual("modified item", listItemToModify.DescriptionOriginal);
        }

        [TestMethod]
        public void Should_Reject_Changes_When_Called_On_Adapter()
        {
            var adapter = new PocoTestAdapter(_tester);

            var listItemToModify = adapter.Items.First();
            listItemToModify.Description = "modified item";

            Assert.IsTrue(adapter.IsChanged);

            adapter.RejectChanges();

            Assert.IsFalse(adapter.IsChanged);
            Assert.AreEqual("Description01", listItemToModify.Description);
            Assert.AreEqual("Description01", listItemToModify.DescriptionOriginal);
        }

    }
}
