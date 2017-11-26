using System;
using System.Linq;
using BPlusTree.DataStructures;
using BPlusTree.Writables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataStructuresUnitTest
{
    [TestClass]
    public class SortedArrayTest
    {
        [TestMethod]
        public void InsertTest()
        {
            const int arraySize = 1_000;
            for (var seed = 0; seed < 1_000; seed++)
            {
                var sortedArray = new SortedArray<WritableInt, WritableInt>(arraySize);
                var rand = new Random(seed);
                var expected = Enumerable.Range(0, arraySize).ToArray();
                var items = expected.OrderBy(i => rand.Next());
                foreach (var item in items)
                    sortedArray.Insert(new WritableInt(item), new WritableInt(item));
                var returned = sortedArray.Select(w => w.Value).ToArray();
                CollectionAssert.AreEqual(expected, returned, $@"
                Expected: {expected.Print()}
                Returned: {returned.Print()}");
            }
        }
    }
}
