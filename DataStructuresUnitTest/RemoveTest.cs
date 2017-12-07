using System;
using System.Collections.Generic;
using System.Linq;
using BPlusTree.Blocks;
using BPlusTree.Writables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BPlusTree.DataStructures;

namespace DataStructuresUnitTest
{
    [TestClass]
    public class RemoveTest
    {
        [TestMethod]
        public void ShiftRightTest()
        {
            var leftItems = Create(1, 3);
            var rightItems = Create(6, 2);
            var parentItems = new[] { new WritableInt(6) };
            var dataBlock = new DataBlock<WritableInt, WritableInt>(5, rightItems);
            var leftSibling = new DataBlock<WritableInt, WritableInt>(5, leftItems);
            DataBlock<WritableInt, WritableInt> rightSibling = null;
            var parent = new IndexBlock<WritableInt>(leftSibling.ByteSize, parentItems);
            var result = BPlusTree<WritableInt, WritableInt>.Shift(parent, 0, ref dataBlock, ref leftSibling, ref rightSibling);
            Assert.IsTrue(result);
            var leftResult = leftSibling.Select(i => i.Value).ToArray();
            var rightResult = dataBlock.Select(i => i.Value).ToArray();
            CollectionAssert.AreEqual(leftResult, new[] { 1, 2 });
            CollectionAssert.AreEqual(rightResult, new[] { 3, 6, 7 });
        }

        [TestMethod]
        public void ShiftLeftTest()
        {
            var leftItems = Create(1, 2);
            var rightItems = Create(6, 3);
            var parentItems = new[] { new WritableInt(6) };
            var dataBlock = new DataBlock<WritableInt, WritableInt>(5, leftItems);
            DataBlock<WritableInt, WritableInt> leftSibling = null;
            var rightSibling = new DataBlock<WritableInt, WritableInt>(5, rightItems);
            var parent = new IndexBlock<WritableInt>(dataBlock.ByteSize, parentItems);
            var result = BPlusTree<WritableInt, WritableInt>.Shift(parent, 0, ref dataBlock, ref leftSibling, ref rightSibling);
            Assert.IsTrue(result);
            var leftResult = dataBlock.Select(i => i.Value).ToArray();
            var rightResult = rightSibling.Select(i => i.Value).ToArray();
            CollectionAssert.AreEqual(leftResult, new[] { 1, 2, 6 });
            CollectionAssert.AreEqual(rightResult, new[] { 7, 8 });
        }

        private IEnumerable<Tuple<WritableInt, WritableInt>> Create(int from, int count)
        {
            var items = new List<Tuple<WritableInt, WritableInt>>(count);
            for (var i = 0; i < count; i++)
            {
                var value = from + i;
                items.Add(Tuple.Create(new WritableInt(value), new WritableInt(value)));
            }
            return items;
        }

        [TestMethod]
        public void DataIndexRemoveAtIndexTest()
        {
            var size = 10;
            for (int i = 0; i < size; i++)
            {
                var index = GetIndex(size);
                index.Remove(i);
                var expected = Enumerable.Range(0, size).Where(v => v != i).ToArray();
                var result = index.Select(v => v.Value).ToArray();
                CollectionAssert.AreEqual(expected, result);
            }
        }

        public SortedIndex<WritableInt> GetIndex(int size)
        {
            var index = new SortedIndex<WritableInt>(size);
            for (int i = 0; i < size; i++)
            {
                index.Insert(new WritableInt(i));
            }
            return index;
        }

        [TestMethod]
        public void MergeTest()
        {
            var leftItems = Create(1, 2).Select(i => i.Item1);
            var rightItems = Create(6, 2).Select(i => i.Item1);
            var left = new SortedIndex<WritableInt>(5, leftItems);
            var right = new SortedIndex<WritableInt>(5, rightItems);
            left.Merge(right);
            var result = left.Select(i => i.Value).ToArray();
            var expected = new[] { 1, 2, 6, 7 };
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
