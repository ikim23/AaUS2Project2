using System;
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
            var reps = 10;
            var numInsertions = 10_000;
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
                File.Delete(testFile);
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
    }
}
