using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class ChangeNotificationTests
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
        public void PropertyChangedEvent_Is_Raised_When_Property_Is_Changed()
        {
            var isRaised = false;

            var adapter = new PocoTestAdapter(_tester);

            adapter.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "TestName")
                {
                    isRaised = true;
                }
            };
            adapter.TestName = "Boris";
            Assert.AreEqual(true, isRaised);
        }


        [TestMethod]
        public void PropertyChangedEvent_Should_Not_Be_Raised_When_Property_Value_Is_Same()
        {
            var isRaised = false;

            var adapter = new PocoTestAdapter(_tester);

            adapter.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "TestId")
                {
                    isRaised = true;
                }
            };
            adapter.TestId = 25;
            Assert.AreEqual(false, isRaised);
        }

        [TestMethod]
        public void Poco_And_Adapter_Collections_Should_Be_In_Sync_After_Removing_Item()
        {
            var adapter = new PocoTestAdapter(_tester);
            var itemToRemove = adapter.Items.Single(ri => ri.Model == _listItem);
            adapter.Items.Remove(itemToRemove);

            CheckCollectionsInSync(adapter);
        }

        [TestMethod]
        public void Poco_And_Adapter_Collections_Should_Be_In_Sync_After_Removing_All_Items()
        {
            var adapter = new PocoTestAdapter(_tester);
            adapter.Items.Clear();

            CheckCollectionsInSync(adapter);
        }


        [TestMethod]
        public void Poco_And_Adapter_Collections_Should_Be_In_Sync_After_Adding_Item()
        {
            _tester.Items.Remove(_listItem);

            var adapter = new PocoTestAdapter(_tester);
            adapter.Items.Add(new PocoListItemAdapter(_listItem));

            CheckCollectionsInSync(adapter);
        }

        private void CheckCollectionsInSync(PocoTestAdapter adapter)
        {
            Assert.AreEqual(_tester.Items.Count, adapter.Items.Count);
            Assert.IsTrue(_tester.Items.All(ti => adapter.Items.Any(ai => ai.Model == ti)));
        }
    }
}
