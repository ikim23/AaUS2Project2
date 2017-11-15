using System;
using System.Linq;
using BPlusTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataStructuresUnitTest
{
    [TestClass]
    public class SortedArrayUnitTest
    {
        [TestMethod]
        public void InsertTest()
        {
            const int size = 10;
            var sortedArray = new SortedArray<WritableInt, WritableInt>(size);
            var rand = new Random();
            var expected = Enumerable.Range(0, size - 1).ToArray();
            var items = expected.OrderBy(i => rand.Next());
            foreach (var item in items)
                sortedArray.Insert(new WritableInt(item), new WritableInt(item));
            CollectionAssert.AreEqual(expected, sortedArray.ToArray());
        }
    }
}
