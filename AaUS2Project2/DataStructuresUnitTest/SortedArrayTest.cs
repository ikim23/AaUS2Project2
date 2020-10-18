using System;
using System.Collections.Generic;
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
        public void SortedBlockInsertTest()
        {
            const int arraySize = 1_000;
            for (var seed = 0; seed < 1_000; seed++)
            {
                var sortedBlock = new SortedBlock<WritableInt, WritableInt>(arraySize);
                var rand = new Random(seed);
                var expected = Enumerable.Range(0, arraySize).ToArray();
                var items = expected.OrderBy(i => rand.Next());
                foreach (var item in items)
                    sortedBlock.Insert(new WritableInt(item), new WritableInt(item));
                var returned = sortedBlock.Select(w => w.Value).ToArray();
                CollectionAssert.AreEqual(expected, returned, $@"
                Expected: {expected.Print()}
                Returned: {returned.Print()}");
            }
        }

        [TestMethod]
        public void SortedBlockSplitTest()
        {
            for (var arraySize = 2; arraySize < 50; arraySize++)
            {
                var sortedBlock = new SortedBlock<WritableInt, WritableInt>(arraySize);
                var i = 0;
                for (; i < arraySize; i++)
                {
                    sortedBlock.Insert(new WritableInt(i), new WritableInt(i));
                }
                var rightSplit = sortedBlock.Split(new WritableInt(i), new WritableInt(i), out var middle);
                var left = sortedBlock.Select(w => w.Value).ToArray();
                var right = rightSplit.Select(w => w.Value).ToArray();
                Assert.AreEqual(right[0], middle.Value);
                var lengthDiff = Math.Abs(left.Length - right.Length);
                Assert.IsTrue(lengthDiff == 0 || lengthDiff == 1);
                var joined = new int[left.Length + right.Length];
                Array.Copy(left, 0, joined, 0, left.Length);
                Array.Copy(right, 0, joined, left.Length, right.Length);
                var expected = Enumerable.Range(0, i + 1).ToArray();
                CollectionAssert.AreEqual(expected, joined);
            }
        }

        [TestMethod]
        public void SortedIndexInsertTest()
        {
            const int arraySize = 1_000;
            for (var seed = 0; seed < 1_000; seed++)
            {
                var sortedIndex = new SortedIndex<WritableInt>(arraySize);
                var rand = new Random(seed);
                var expected = Enumerable.Range(0, arraySize).ToArray();
                var items = expected.OrderBy(i => rand.Next());
                foreach (var item in items)
                    sortedIndex.Insert(new WritableInt(item));
                var returned = sortedIndex.Select(w => w.Value).ToArray();
                CollectionAssert.AreEqual(expected, returned, $@"
                Expected: {expected.Print()}
                Returned: {returned.Print()}");
            }
        }

        [TestMethod]
        public void SortedIndexSplitTest()
        {
            for (var arraySize = 2; arraySize < 50; arraySize++)
            {
                var sortedIndex = new SortedIndex<WritableInt>(arraySize);
                var i = 0;
                for (; i < arraySize; i++)
                {
                    sortedIndex.Insert(new WritableInt(i));
                }
                var rightSplit = sortedIndex.Split(new WritableInt(i), out var middle);
                var left = sortedIndex.Select(w => w.Value).ToArray();
                var right = rightSplit.Select(w => w.Value).ToArray();
                var lengthDiff = Math.Abs(left.Length - right.Length);
                Assert.IsTrue(lengthDiff == 0 || lengthDiff == 1);
                var joined = new int[left.Length + right.Length + 1];
                Array.Copy(left, 0, joined, 0, left.Length);
                joined[left.Length] = middle.Value;
                Array.Copy(right, 0, joined, left.Length + 1, right.Length);
                var expected = Enumerable.Range(0, i + 1).ToArray();
                CollectionAssert.AreEqual(expected, joined);
            }
        }

        [TestMethod]
        public void InsertionIndexTest()
        {
            var size = 6;
            for (var seed = 0; seed < 1_000_000; seed++)
            {
                var rand = new Random(seed);
                var set = new HashSet<int>();
                while (set.Count < size)
                {
                    set.Add(rand.Next());
                }
                var items = set.ToList();
                var sortedIndex = new SortedIndex<WritableInt>(size);
                foreach (var item in items)
                {
                    var itemToInsert = new WritableInt(item);
                    var insertionIndex = sortedIndex.FindInsertionIndex(itemToInsert);
                    var keys = sortedIndex.Items;
                    for (var i = 0; i < insertionIndex; i++)
                    {
                        Assert.IsTrue(keys[i].CompareTo(itemToInsert) < 0);
                    }
                    for (var i = insertionIndex; i < sortedIndex.Count; i++)
                    {
                        Assert.IsTrue(keys[i].CompareTo(itemToInsert) > 0);
                    }
                    sortedIndex.Insert(itemToInsert);
                    Assert.AreEqual(itemToInsert.Value, sortedIndex.Items[insertionIndex].Value);
                }
                var arr = sortedIndex.Items;
                for (var i = 0; i < size - 1; i++)
                {
                    Assert.IsTrue(arr[i].Value < arr[i + 1].Value);
                }
            }
        }
    }
}
