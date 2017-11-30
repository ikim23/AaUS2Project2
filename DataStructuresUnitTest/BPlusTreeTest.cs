using System;
using System.IO;
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
                //Console.WriteLine(val);
                Assert.AreEqual(value, val.Value);
                value++;
            }
            for (var i = 0; i < reps; i++)
            {
                //Console.WriteLine(i);
                //try
                //{
                var result = tree.Find(new WritableInt(i));
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(i);
                //}
                Assert.AreEqual(i, result.Value);
            }
            tree.Dispose();
            //File.Delete(testFile);
        }
    }
}
