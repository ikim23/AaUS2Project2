using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BPlusTree;
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
            var numInsertions = 100_000;
            var testFile = $"{DateTime.Now.Ticks}.bin";
            var tree = new BPlusTree<WritableInt, WritableInt>(5, testFile);
            for (var i = 0; i < numInsertions; i++)
            {
                tree.Insert(new WritableInt(i), new WritableInt(i));
            }
            var value = 0;
            foreach (var val in tree.InOrder())
            {
                Assert.AreEqual(value, val.Value);
                value++;
            }
            // test if in order match
            var expected = Enumerable.Range(0, numInsertions).ToList();
            var returned = tree.InOrder().Select(e => e.Value).ToList();
            Assert.AreEqual(expected.Count, returned.Count);
            CollectionAssert.AreEqual(expected, returned);
            // test find for every item
            for (var i = 0; i < numInsertions; i++)
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
            var reps = 20;
            var numInsertions = 1_000;
            for (var rep = 0; rep < reps; rep++)
            {
                var rand = new Random(rep);
                var items = Utils.RandomUniqueList(rand, numInsertions);
                var testFile = $"{DateTime.Now.Ticks}.bin";
                var blockSize = 5 + rep;
                var tree = new BPlusTree<WritableInt, WritableInt>(blockSize, testFile);
                for (var index = 0; index < items.Count; index++)
                {
                    var item = items[index];
                    tree.Insert(item, item);
                    //// check in order values
                    //var expected = items.Take(index + 1).Select(e => e.Value).ToList();
                    //expected.Sort();
                    //var returned = tree.InOrder().Select(e => e.Value).ToList();
                    //Assert.AreEqual(expected.Count, returned.Count);
                    //CollectionAssert.AreEqual(expected, returned);
                    //// find all inserted values
                    //for (var i = 0; i <= index; i++)
                    //{
                    //    var searchFor = items[i];
                    //    var found = tree.Find(searchFor);
                    //    Assert.AreEqual(searchFor.Value, found.Value);
                    //}
                }
                // test if in order match
                var expected = items.Select(e => e.Value).ToList();
                expected.Sort();
                var returned = tree.InOrder().Select(e => e.Value).ToList();
                Assert.AreEqual(expected.Count, returned.Count);
                CollectionAssert.AreEqual(expected, returned, $@"
                Expected: {string.Join(",", expected)}
                Returned: {string.Join(",", returned)}");
                // test find for every item
                foreach (var item in expected)
                {
                    var result = tree.Find(new WritableInt(item));
                    Assert.AreEqual(item, result.Value);
                }
                tree.Dispose();
                //File.Delete(testFile);
            }
        }

        [TestMethod]
        public void PatientInsertTest()
        {
            var blockSize = 8;
            var numInsertions = 1_000;
            var rand = new Random(100);
            var patients = Utils.RandomUniquePatients(rand, numInsertions);
            var testFile = $"{DateTime.Now.Ticks}.bin";
            var tree = new BPlusTree<WritableInt, Patient>(blockSize, testFile);
            for (var index = 0; index < patients.Count; index++)
            {
                var patient = patients[index];
                tree.Insert(new WritableInt(patient.CardId), patient);
            }
            // test if in order match
            var expected = patients.Select(p => p.CardId).ToList();
            expected.Sort();
            var returned = tree.InOrder().Select(p => p.CardId).ToList();
            Assert.AreEqual(expected.Count, returned.Count);
            CollectionAssert.AreEqual(expected, returned, $@"
                Expected: {string.Join(",", expected)}
                Returned: {string.Join(",", returned)}");
            // test find for every item
            foreach (var expectedPatient in patients)
            {
                var foundPatient = tree.Find(new WritableInt(expectedPatient.CardId));
                Utils.AssertPatients(expectedPatient, foundPatient);
            }
            tree.Dispose();
            //File.Delete(testFile);
        }

        [TestMethod]
        public void RemoveTest()
        {
            var numInsertions = 1_00;
            var testFile = $"{DateTime.Now.Ticks}.bin";
            var tree = new BPlusTree<WritableInt, WritableInt>(10, testFile);
            for (var i = 0; i < numInsertions; i++)
            {
                //Console.WriteLine(i);
                tree.Insert(new WritableInt(i), new WritableInt(i));
            }
            // check if all present
            for (var i = 0; i < numInsertions; i++)
            {
                tree.Find(new WritableInt(i));
            }
            // delete 
            for (var i = 0; i < numInsertions; i++)
            {
                tree.Remove(new WritableInt(i));
                // test search
                for (var index = i + 1; index < numInsertions; index++)
                {
                    tree.Find(new WritableInt(index));
                }
                // test if in order match
                var expected = Enumerable.Range(i + 1, numInsertions - (i + 1)).ToList();
                var returned = tree.InOrder().Select(e => e.Value).ToList();
                //Console.WriteLine(string.Join(",", expected));
                //Console.WriteLine(string.Join(",", returned));
                //Console.WriteLine();
                Assert.AreEqual(expected.Count, returned.Count);
                CollectionAssert.AreEqual(expected, returned, $@"
                Expected: {string.Join(",", expected)}
                Returned: {string.Join(",", returned)}");
            }
            tree.Dispose();
            File.Delete(testFile);
        }

        [TestMethod]
        public void InsertRemoveTest()
        {
            var reps = 20;
            var numInsertions = 1_000;
            for (var rep = 0; rep < reps; rep++)
            {
                var blockSize = 5 + rep;
                var rand = new Random(blockSize);
                var testFile = $"{DateTime.Now.Ticks}.bin";
                var tree = new BPlusTree<WritableInt, WritableInt>(blockSize, testFile);
                var items = Utils.RandomUniqueList(rand, numInsertions);
                foreach (var key in items)
                {
                    tree.Insert(key, key);
                }
                // check if all keys are present
                for (var i = 0; i < numInsertions; i++)
                {
                    tree.Find(new WritableInt(i));
                }
                // random delete
                var removeItems = items.OrderBy(v => rand.Next()).ToList();
                while (removeItems.Count > 0)
                {
                    var removeItem = removeItems[0];
                    removeItems.RemoveAt(0);
                    try
                    {
                        tree.Remove(removeItem);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Remove: {removeItem}, Size: {removeItems.Count}, BlockSize: {blockSize}");
                        Console.WriteLine(e);
                        throw e;
                    }
                    // find all
                    foreach (var notRemovedItem in removeItems)
                    {
                        tree.Find(notRemovedItem);
                    }
                    // check in order
                    var expected = removeItems.Select(i => i.Value).ToList();
                    expected.Sort();
                    var returned = tree.InOrder().Select(i => i.Value).ToList();
                    CollectionAssert.AreEqual(expected, returned, $@"
                        Expected: {string.Join(",", expected)}
                        Returned: {string.Join(",", returned)}");
                }
                tree.Dispose();
                // test if all block were removed
                var factory = Factory.Create(testFile);
                Assert.AreEqual(factory.ControlBlockByteSize, factory.Length());
                factory.Dispose();
                File.Delete(testFile);
            }
        }

        [TestMethod]
        public void RandomInsertRemoveTest()
        {
            var reps = 20;
            var numInsertions = 1_000;
            var numOperations = 500;
            for (var rep = 0; rep < reps; rep++)
            {
                var blockSize = 5 + rep;
                var rand = new Random(blockSize);
                var testFile = $"{DateTime.Now.Ticks}.bin";
                var tree = new BPlusTree<WritableInt, WritableInt>(blockSize, testFile);
                var items = Utils.RandomUniqueList(rand, numInsertions);
                foreach (var key in items)
                {
                    tree.Insert(key, key);
                }
                // check if all keys are present
                for (var i = 0; i < numInsertions; i++)
                {
                    tree.Find(new WritableInt(i));
                }
                // random delete/insert
                var existingItems = items.OrderBy(v => rand.Next()).ToList();
                var removedItems = new List<WritableInt>();
                for (var operation = 0; operation < numOperations; operation++)
                {
                    var insert = rand.NextDouble();
                    if (removedItems.Count > 0 && insert < 0.4)
                    {
                        var removedItem = removedItems[0];
                        removedItems.RemoveAt(0);
                        if (operation > 400) Console.WriteLine($"Op: {operation} Insert: {removedItem} Size: {existingItems.Count} BlockSize: {blockSize}");
                        var val = removedItem.Value;
                        //Console.WriteLine($"Insert {removedItem}");
                        tree.Insert(removedItem, removedItem);
                        existingItems.Add(removedItem);
                    }
                    else
                    {
                        var itemToRemove = existingItems[0];
                        existingItems.RemoveAt(0);
                        removedItems.Add(itemToRemove);
                        try
                        {
                            if (operation > 400) Console.WriteLine($"Op: {operation} Remove: {itemToRemove} Size: {existingItems.Count} BlockSize: {blockSize}");
                            var val = itemToRemove.Value;
                            tree.Remove(itemToRemove);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Op: {operation} Remove: {itemToRemove} Size: {existingItems.Count} BlockSize: {blockSize}");
                            Console.WriteLine(e);
                            throw e;
                        }
                    }
                    // find all
                    foreach (var notRemovedItem in existingItems)
                    {
                        var val = notRemovedItem.Value;
                        tree.Find(notRemovedItem);
                    }
                    // check in order
                    var expected = existingItems.Select(i => i.Value).ToList();
                    expected.Sort();
                    var returned = tree.InOrder().Select(i => i.Value).ToList();
                    CollectionAssert.AreEqual(expected, returned, $@"
                        Expected: {string.Join(",", expected)}
                        Returned: {string.Join(",", returned)}");
                }
                while (existingItems.Count > 0)
                {
                    var itemToRemove = existingItems[0];
                    existingItems.RemoveAt(0);
                    try
                    {
                        tree.Remove(itemToRemove);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Cleanup Remove: {itemToRemove} Size: {existingItems.Count} BlockSize: {blockSize}");
                        Console.WriteLine(e);
                        throw e;
                    }
                    // find all
                    foreach (var notRemovedItem in existingItems)
                    {
                        var val = notRemovedItem.Value;
                        tree.Find(notRemovedItem);
                    }
                    // check in order
                    var expected = existingItems.Select(i => i.Value).ToList();
                    expected.Sort();
                    var returned = tree.InOrder().Select(i => i.Value).ToList();
                    CollectionAssert.AreEqual(expected, returned, $@"
                        Expected: {string.Join(",", expected)}
                        Returned: {string.Join(",", returned)}");
                }
                tree.Dispose();
                // test if all block were removed
                var factory = Factory.Create(testFile);
                Assert.AreEqual(factory.ControlBlockByteSize, factory.Length());
                factory.Dispose();
                File.Delete(testFile);
            }
        }
    }
}
