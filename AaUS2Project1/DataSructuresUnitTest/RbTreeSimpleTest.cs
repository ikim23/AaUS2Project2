using System;
using System.Linq;
using DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataSructuresUnitTest
{
    [TestClass]
    public class RbTreeSimpleTest
    {
        [TestMethod]
        public void InOrderTest()
        {
            var tree = new RbTree<int, int>();
            for (var key = 1; key <= 1000; key++)
                tree.Insert(key, key);
            var previousKey = Int32.MinValue;
            foreach (var key in tree.InOrder())
            {
                Assert.IsTrue(previousKey < key, "Nodes are not ordered properly");
                previousKey = key;
            }
        }

        [TestMethod]
        public void BlackHeightTest()
        {
            var keys = new[] { 4, 7, 12, 15, 3, 5, 14, 18, 16, 17, 100, 60, 0, 6, 10, 8 };
            var tree = new RbTree<int, int>();
            foreach (var key in keys)
                tree.Insert(key, key);
            var expectedBlackHeight = 4;
            var blackHeight = tree.CheckBlackHeight();
            Assert.AreEqual(expectedBlackHeight, blackHeight);
        }

        public void ShiftTest(int[] keys, int[] expectedLevelOrder)
        {
            var tree = new RbTree<int, int>();
            foreach (var key in keys)
                tree.Insert(key, key);
            var levelOrder = tree.LevelOrder().ToArray();
            CollectionAssert.AreEqual(expectedLevelOrder, levelOrder, $@"
                Expected: {expectedLevelOrder.Format()}]
                Returned: {levelOrder.Format()}");
        }

        [TestMethod]
        public void LeftShiftTest1()
        {
            var keys = new[] { 13, 14, 16, 20 };
            var expectedLevelOrder = new[] { 14, 13, 16, 20 };
            ShiftTest(keys, expectedLevelOrder);
        }

        [TestMethod]
        public void LeftShiftTest2()
        {
            var keys = new[] { 16, 13, 14 };
            var expectedLevelOrder = new[] { 14, 13, 16 };
            ShiftTest(keys, expectedLevelOrder);
        }

        [TestMethod]
        public void RightShiftTest1()
        {
            var keys = new[] { 16, 14, 13, 10 };
            var expectedLevelOrder = new[] { 14, 13, 16, 10 };
            ShiftTest(keys, expectedLevelOrder);
        }

        [TestMethod]
        public void RightShiftTest2()
        {
            var keys = new[] { 13, 16, 14 };
            var expectedLevelOrder = new[] { 14, 13, 16 };
            ShiftTest(keys, expectedLevelOrder);
        }

        [TestMethod]
        public void InsertTest()
        {
            var items = new[] { 4, 7, 12, 15, 3, 5, 14, 18, 16, 17 };
            var expectedLevelOrder = new[] { 14, 7, 16, 4, 12, 15, 18, 3, 5, 17 };
            var tree = new RbTree<int, int>();
            foreach (var item in items)
                tree.Insert(item, item);
            var levelOrder = tree.LevelOrder().ToArray();
            CollectionAssert.AreEqual(expectedLevelOrder, levelOrder, $@"
                Expected: {expectedLevelOrder.Format()}]
                Returned: {levelOrder.Format()}");
        }

        [TestMethod]
        public void RemoveTestCase4And1()
        {
            var keys = new[] { 4, 7, 12, 15, 3, 5, 14, 18, 16, 17, 100, 60, 0, 6, 10, 8 };
            var expectedLevelOrder = new[] { 14, 7, 18, 4, 10, 16, 100, 3, 5, 8, 12, 17, 60, 0, 6 };
            var tree = new RbTree<int, int>();
            foreach (var key in keys)
                tree.Insert(key, key);
            tree.Print();
            // remove 15: case 4 -> case 1 (15 is leaf => 15 is double black)
            tree.Remove(15);
            tree.Print();
            var levelOrder = tree.LevelOrder().ToArray();
            CollectionAssert.AreEqual(expectedLevelOrder, levelOrder, $@"
                Expected: {expectedLevelOrder.Format()}]
                Returned: {levelOrder.Format()}");
            Assert.IsTrue(tree.Check());
        }

        [TestMethod]
        public void RemoveTestCase3And2()
        {
            var keys = new[] { 4, 7, 12, 15, 3, 5, 14, 18, 16, 17, 100, 60, 0, 6, 10, 8 };
            var expectedLevelOrder = new[] { 14, 7, 17, 4, 10, 15, 60, 3, 5, 8, 12, 18, 100, 0, 6 };
            var tree = new RbTree<int, int>();
            foreach (var key in keys)
                tree.Insert(key, key);
            tree.Print();
            // remove 16: case 3 -> case 2 (17 is double black)
            tree.Remove(16);
            var levelOrder = tree.LevelOrder().ToArray();
            CollectionAssert.AreEqual(expectedLevelOrder, levelOrder, $@"
                Expected: {expectedLevelOrder.Format()}]
                Returned: {levelOrder.Format()}");
            Assert.IsTrue(tree.Check());
        }

        [TestMethod]
        public void GetIntervalTest1()
        {
            // in order
            var keys = new[] { 4, 7, 12, 15, 3, 5, 14, 18, 16, 17, 100, 60, 0, 6, 10, 8 };
            var expectedInterval = new[] { 0, 3, 4, 5, 6, 7, 8, 10, 12, 14, 15, 16, 17, 18, 60, 100 };
            var tree = new RbTree<int, int>();
            foreach (var key in keys)
                tree.Insert(key, key);
            tree.Print();
            var interval = tree.GetInterval(0, 100).ToArray();
            CollectionAssert.AreEqual(expectedInterval, interval, $@"
                Expected: {expectedInterval.Format()}]
                Returned: {interval.Format()}");
            Assert.IsTrue(tree.Check());
        }

        [TestMethod]
        public void GetIntervalTest2()
        {
            // left subtree
            var keys = new[] { 4, 7, 12, 15, 3, 5, 14, 18, 16, 17, 100, 60, 0, 6, 10, 8 };
            var expectedInterval = new[] { 5, 6, 7, 8, 10 };
            var tree = new RbTree<int, int>();
            foreach (var key in keys)
                tree.Insert(key, key);
            tree.Print();
            var interval = tree.GetInterval(5, 10).ToArray();
            CollectionAssert.AreEqual(expectedInterval, interval, $@"
                Expected: {expectedInterval.Format()}]
                Returned: {interval.Format()}");
            Assert.IsTrue(tree.Check());
        }

        [TestMethod]
        public void GetIntervalTest3()
        {
            // right subtree
            var keys = new[] { 4, 7, 12, 15, 3, 5, 14, 18, 16, 17, 100, 60, 0, 6, 10, 8 };
            var expectedInterval = new[] { 15, 16, 17 };
            var tree = new RbTree<int, int>();
            foreach (var key in keys)
                tree.Insert(key, key);
            tree.Print();
            var interval = tree.GetInterval(15, 17).ToArray();
            CollectionAssert.AreEqual(expectedInterval, interval, $@"
                Expected: {expectedInterval.Format()}]
                Returned: {interval.Format()}");
            Assert.IsTrue(tree.Check());
        }

        [TestMethod]
        public void GetIntervalTest4()
        {
            // both subtrees
            var keys = new[] { 4, 7, 12, 15, 3, 5, 14, 18, 16, 17, 100, 60, 0, 6, 10, 8 };
            var expectedInterval = new[] { 8, 10, 12, 14, 15, 16, 17, 18, 60 };
            var tree = new RbTree<int, int>();
            foreach (var key in keys)
                tree.Insert(key, key);
            tree.Print();
            var interval = tree.GetInterval(8, 60).ToArray();
            CollectionAssert.AreEqual(expectedInterval, interval, $@"
                Expected: {expectedInterval.Format()}]
                Returned: {interval.Format()}");
            Assert.IsTrue(tree.Check());
        }

        [TestMethod]
        public void GetIntervalTest5()
        {
            var keys = new[] { 4, 7, 12, 15, 3, 5, 14, 18, 16, 17, 100, 60, 0, 6, 10, 8 };
            var expectedInterval = new[] { 10, 12, 14, 15, 16, 17, 18, 60 };
            var tree = new RbTree<int, int>();
            foreach (var key in keys)
                tree.Insert(key, key);
            tree.Print();
            var interval = tree.GetInterval(9, 65).ToArray();
            CollectionAssert.AreEqual(expectedInterval, interval, $@"
                Expected: {expectedInterval.Format()}]
                Returned: {interval.Format()}");
            Assert.IsTrue(tree.Check());
        }
    }
}
