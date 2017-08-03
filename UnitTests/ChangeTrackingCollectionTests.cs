using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using SaibaMvvmAdapter;

namespace UnitTests
{
    [TestClass]
    public class ChangeTrackingCollectionTests
    {
        private List<PocoListItemAdapter> _listItems;

        [TestInitialize]
        public void Initialize()
        {
            _listItems = new List<PocoListItemAdapter>
           {
               new PocoListItemAdapter( new PocoListItem { Id = 1, Title = "TestItem01", Description = "Description01" }),
               new PocoListItemAdapter( new PocoListItem { Id = 2, Title = "TestItem02", Description = "Description02" })
           };
        }

        [TestMethod]
        public void Should_Track_Added_Items()
        {

            var listItemToAdd = new PocoListItemAdapter(new PocoListItem());

            var c = new ChangeTrackingCollection<PocoListItemAdapter>(_listItems);
            Assert.AreEqual(2, c.Count);
            Assert.IsFalse(c.IsChanged);

            c.Add(listItemToAdd);
            Assert.AreEqual(3, c.Count);
            Assert.AreEqual(1, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(listItemToAdd, c.AddedItems.First());
            Assert.IsTrue(c.IsChanged);

            c.Remove(listItemToAdd);
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.IsFalse(c.IsChanged);
        }

        [TestMethod]
        public void Should_Track_Removed_Items()
        {
            var listItemToRemove = _listItems.First();
            var c = new ChangeTrackingCollection<PocoListItemAdapter>(_listItems);
            Assert.AreEqual(2, c.Count);
            Assert.IsFalse(c.IsChanged);

            c.Remove(listItemToRemove);
            Assert.AreEqual(1, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(1, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(listItemToRemove, c.RemovedItems.First());
            Assert.IsTrue(c.IsChanged);

            c.Add(listItemToRemove);
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.IsFalse(c.IsChanged);
        }

        [TestMethod]
        public void Should_Track_Modified_Item()
        {
            var listItemToModify = _listItems.First();
            var c = new ChangeTrackingCollection<PocoListItemAdapter>(_listItems);
            Assert.AreEqual(2, c.Count);
            Assert.IsFalse(c.IsChanged);

            listItemToModify.Description = "modified entry";
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(1, c.ModifiedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.IsTrue(c.IsChanged);

            listItemToModify.Description = "Description01";
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.IsFalse(c.IsChanged);
        }

        [TestMethod]
        public void Should_Not_Track_Added_Item_As_Modified()
        {
            var listItemToAdd = new PocoListItemAdapter(new PocoListItem());

            var c = new ChangeTrackingCollection<PocoListItemAdapter>(_listItems);
            c.Add(listItemToAdd);
            listItemToAdd.Description = "modified entry";
            Assert.IsTrue(listItemToAdd.IsChanged);
            Assert.AreEqual(3, c.Count);
            Assert.AreEqual(1, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.IsTrue(c.IsChanged);
        }

        [TestMethod]
        public void Should_Not_Track_Removed_Item_As_Modified()
        {
            var listItemToModifyAndRemove = _listItems.First();

            var c = new ChangeTrackingCollection<PocoListItemAdapter>(_listItems);
            listItemToModifyAndRemove.Description = "modified entry";
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(1, c.ModifiedItems.Count);
            Assert.AreEqual(listItemToModifyAndRemove, c.ModifiedItems.First());
            Assert.IsTrue(c.IsChanged);

            c.Remove(listItemToModifyAndRemove);
            Assert.AreEqual(1, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(1, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(listItemToModifyAndRemove, c.RemovedItems.First());
            Assert.IsTrue(c.IsChanged);
        }

        [TestMethod]
        public void Should_Accept_Changes()
        {
            var listItemToAdd = new PocoListItemAdapter(new PocoListItem { Id = 3, Title = "TestItem03", Description = "Description03" });
            var listItemToModify = _listItems.First();
            var listItemToRemove = _listItems.Skip(1).First();


            var c = new ChangeTrackingCollection<PocoListItemAdapter>(_listItems);

            c.Add(listItemToAdd);
            c.Remove(listItemToRemove);
            listItemToModify.Description = "modified item";
            Assert.AreEqual("Description01", listItemToModify.DescriptionOriginal);

            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(1, c.AddedItems.Count);
            Assert.AreEqual(1, c.ModifiedItems.Count);
            Assert.AreEqual(1, c.RemovedItems.Count);

            c.AcceptChanges();

            Assert.AreEqual(2, c.Count);
            Assert.IsTrue(c.Contains(listItemToModify));
            Assert.IsTrue(c.Contains(listItemToAdd));

            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
                   
            Assert.IsFalse(listItemToModify.IsChanged);
            Assert.AreEqual("modified item", listItemToModify.Description);
            Assert.AreEqual("modified item", listItemToModify.DescriptionOriginal);

            Assert.IsFalse(c.IsChanged);
        }

        [TestMethod]
        public void Should_Reject_Changes()
        {
            var listItemToAdd = new PocoListItemAdapter(new PocoListItem { Id = 3, Title = "TestItem03", Description = "Description03" });
            var listItemToModify = _listItems.First();
            var listItemToRemove = _listItems.Skip(1).First();


            var c = new ChangeTrackingCollection<PocoListItemAdapter>(_listItems);

            c.Add(listItemToAdd);
            c.Remove(listItemToRemove);
            listItemToModify.Description = "modified item";
            Assert.AreEqual("Description01", listItemToModify.DescriptionOriginal);

            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(1, c.AddedItems.Count);
            Assert.AreEqual(1, c.ModifiedItems.Count);
            Assert.AreEqual(1, c.RemovedItems.Count);

            c.RejectChanges();

            Assert.AreEqual(2, c.Count);
            Assert.IsTrue(c.Contains(listItemToModify));
            Assert.IsTrue(c.Contains(listItemToRemove));

            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);

            Assert.IsFalse(listItemToModify.IsChanged);
            Assert.AreEqual("Description01", listItemToModify.Description);
            Assert.AreEqual("Description01", listItemToModify.DescriptionOriginal);

            Assert.IsFalse(c.IsChanged);
        }

        [TestMethod]
        public void Should_Reject_Changes_With_Modified_And_Removed_Items()
        {

            var listItem = _listItems.First();

            var c = new ChangeTrackingCollection<PocoListItemAdapter>(_listItems);

            listItem.Description = "modified item";
            c.Remove(listItem);
            Assert.AreEqual("Description01", listItem.DescriptionOriginal);
            Assert.AreEqual(1, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(1, c.RemovedItems.Count);
                   
            c.RejectChanges();

            Assert.AreEqual(2, c.Count);
            Assert.IsTrue(c.Contains(listItem));

            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);

            Assert.IsFalse(listItem.IsChanged);
            Assert.AreEqual("Description01", listItem.Description);
            Assert.AreEqual("Description01", listItem.DescriptionOriginal);

            Assert.IsFalse(c.IsChanged);
        }

    }
}
