using System;
using System.Collections.Generic;
using DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataSructuresUnitTest
{
    [TestClass]
    public class RbTreeTest
    {
        private const int Seed = 1;
        private const int NodeCount = 10_000;

        [TestMethod]
        public void InsertTest()
        {
            var keys = U.UniqueList(NodeCount);
            var tree = new RbTree<int, int>();
            foreach (var key in keys)
            {
                tree.Insert(key, key);
                Assert.IsTrue(tree.Check(), $"Failed at key: {key}");
                Assert.AreEqual(key, tree.Find(key), $"Key: {key} was not found");
            }
        }

        [TestMethod]
        public void RemoveTest()
        {
            var keys = U.UniqueList(NodeCount);
            var tree = new RbTree<int, int>();
            foreach (var key in keys)
                tree.Insert(key, key);
            while (keys.Count > 0)
            {
                var key = keys.Pop();
                tree.Remove(key);
                Assert.IsTrue(tree.Check(), $"Failed at key: {key}");
            }
        }

        [TestMethod]
        public void RandomInsertRemoveTest()
        {
            var rand = new Random(Seed);
            var keys = U.UniqueList(NodeCount, Seed);
            var removed = new List<int>();

            var tree = new RbTree<int, int>();
            foreach (var key in keys)
                tree.Insert(key, key);

            const int operations = NodeCount / 10;
            for (var i = 0; i < operations; i++)
            {
                int key;
                string operation;
                if (removed.Count > 0 && rand.Next() < 0.4)
                {
                    operation = "Insert";
                    key = removed.Pop();
                    tree.Insert(key, key);
                    keys.Add(key);
                    foreach (var k in keys)
                        Assert.AreEqual(tree.Find(k), k);
                }
                else
                {
                    operation = "Remove";
                    key = keys.Pop();
                    tree.Remove(key);
                    removed.Add(key);
                    foreach (var k in keys)
                        Assert.AreEqual(tree.Find(k), k);
                }
                Assert.IsTrue(tree.Check(), $"Failed at {operation}: {key}");
            }
        }

        [TestMethod]
        public void RandomInsertRemoveLongTest()
        {
            var nodeCount = 1_000;
            for (var seed = 0; seed < 100; seed++)
            {
                var rand = new Random(seed);
                var keys = U.UniqueList(nodeCount, seed);
                var removed = new List<int>();

                var tree = new RbTree<int, int>();
                foreach (var key in keys)
                    tree.Insert(key, key);

                const int operations = NodeCount / 10;
                for (var i = 0; i < operations; i++)
                {
                    int key;
                    string operation;
                    if (removed.Count > 0 && rand.Next() < 0.4)
                    {
                        operation = "Insert";
                        key = removed.Pop();
                        tree.Insert(key, key);
                        keys.Add(key);
                        foreach (var k in keys)
                            Assert.AreEqual(tree.Find(k), k);
                    }
                    else
                    {
                        operation = "Remove";
                        key = keys.Pop();
                        tree.Remove(key);
                        removed.Add(key);
                        foreach (var k in keys)
                            Assert.AreEqual(tree.Find(k), k);
                    }
                    Assert.IsTrue(tree.Check(), $"Failed at {operation}: {key}");
                }
            }
        }
    }
}
