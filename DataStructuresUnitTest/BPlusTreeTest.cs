using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BPlusTree.DataStructures;
using BPlusTree.Writables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataStructuresUnitTest
{
    [TestClass]
    public class BPlusTreeTest
    {
        [TestMethod]
        public void InsertTest()
        {
            var testFile = $"{DateTime.Now.Ticks}.bin";
            var reps = 100_000;
            var tree = new BPlusTree<WritableInt, WritableInt>(5, testFile);
            for (var i = 0; i < reps; i++)
            {
                tree.Insert(new WritableInt(i), new WritableInt(i));
            }
            var value = 0;
            foreach (var val in tree.InOrder())
            {
                Assert.AreEqual(value, val.Value);
                value++;
            }
            for (var i = 0; i < reps; i++)
            {
                var result = tree.Find(new WritableInt(i));
                Assert.AreEqual(i, result.Value);
            }
            tree.Dispose();
            File.Delete(testFile);
        }

        [TestMethod]
        public void RandomInsertTest()
        {
            var reps = 1;
            var blockSize = 5;
            var numInsertions = 52; //150; //133
            for (var rep = 0; rep < reps; rep++)
            {
                var rand = new Random(rep);
                var items = Utils.RandomUniqueList(rand, numInsertions);
                var testFile = $"{DateTime.Now.Ticks}.bin";
                var tree = new BPlusTree<WritableInt, WritableInt>(blockSize, testFile);
                for (var index = 0; index < items.Count; index++)
                {
                    var item = items[index];
                    tree.Insert(item, item);

                    Console.WriteLine($"Inserted value: {item.Value}");

                    // check in order values
                    var expected = items.Take(index + 1).Select(node => node.Value).ToList();
                    expected.Sort();
                    var returned = tree.InOrder().Select(node => node.Value).ToList();
                    Assert.AreEqual(expected.Count, returned.Count);
                    CollectionAssert.AreEqual(expected, returned);

                    if (item.Value == 50)
                    {
                        //tree.CheckInternalNodeOrder();
                        tree.Print();
                    }
                    // find all inserted values
                    for (var i = 0; i <= index; i++)
                    {
                        var searchFor = items[i];
                        var found = tree.Find(searchFor);
                        Assert.AreEqual(searchFor.Value, found.Value, $"Tree count: {index + 1}");
                    }
                }
                // remove tree
                tree.Dispose();
                File.Delete(testFile);
            }
        }
    }
}
