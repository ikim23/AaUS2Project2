using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BPlusTree;
using BPlusTree.Blocks;
using BPlusTree.DataStructures;
using BPlusTree.Writables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataStructuresUnitTest
{
    [TestClass]
    public class BlockFactoryTest
    {
        [TestMethod]
        public void ReadWriteBlockTest()
        {
            var testFile = $"{DateTime.Now.Ticks}.bin";
            var random = new Random(1);
            var numBlocks = 10;
            var controlBlockSize = 18;
            var blockSize = Utils.RandomDataBlock(random).ByteSize;
            // write blocks
            var blocks = new List<DataBlock<WritableInt, Patient>>(numBlocks);
            using (var factory = new BlockFactory<WritableInt, Patient>(Utils.RandomDataBlockSize, testFile))
            {
                for (var i = 0; i < numBlocks; i++)
                {
                    var block = Utils.RandomDataBlock(random);
                    block.Address = factory.GetFreeAddress();
                    factory.WriteBlock(block);
                    blocks.Add(block);
                }
            }
            // read blocks
            using (var factory = new BlockFactory<WritableInt, Patient>(Utils.RandomDataBlockSize, testFile))
            {
                for (var i = 0; i < numBlocks; i++)
                {
                    var block = blocks[i];
                    var address = controlBlockSize + blockSize * i;
                    var loadedBlock = (DataBlock<WritableInt, Patient>)factory.ReadBlock(address);
                    Utils.AssertDataBlocks(block, loadedBlock);
                }
            }
            File.Delete(testFile);
        }

        [TestMethod]
        public void DeleteBlockTest()
        {
            var testFile = $"{DateTime.Now.Ticks}.bin";
            var random = new Random(1);
            var numBlocks = 10;
            var controlBlockSize = 18;
            var blockSize = Utils.RandomDataBlock(random).ByteSize;
            // write blocks
            var blocks = new List<DataBlock<WritableInt, Patient>>(numBlocks);
            using (var factory = new BlockFactory<WritableInt, Patient>(Utils.RandomDataBlockSize, testFile))
            {
                for (var i = 0; i < numBlocks; i++)
                {
                    var block = Utils.RandomDataBlock(random);
                    block.Address = factory.GetFreeAddress();
                    factory.WriteBlock(block);
                    blocks.Add(block);
                }
            }
            // remove all odd blocks
            var emptyAddrs = new List<long>();
            using (var factory = new BlockFactory<WritableInt, Patient>(Utils.RandomDataBlockSize, testFile))
            {
                for (var i = 0; i < numBlocks; i = i + 2)
                {
                    var block = blocks[i];
                    var loadedBlock = (DataBlock<WritableInt, Patient>)factory.ReadBlock(controlBlockSize + blockSize * i);
                    Utils.AssertDataBlocks(block, loadedBlock);
                    factory.RemoveBlock(loadedBlock);
                    emptyAddrs.Add(loadedBlock.Address);
                }
            }
            // read blocks
            using (var factory = new BlockFactory<WritableInt, Patient>(Utils.RandomDataBlockSize, testFile))
            {
                for (var i = 0; i < numBlocks; i++)
                {
                    if (i % 2 == 0)
                    {
                        var emptyIdx = i / 2;
                        var loadedBlock = (EmptyBlock)factory.ReadBlock(controlBlockSize + blockSize * i);
                        Assert.AreEqual(emptyAddrs[emptyIdx], loadedBlock.Address);
                        if (emptyIdx == 0)
                        {
                            Assert.AreEqual(emptyAddrs[emptyIdx + 1], loadedBlock.PrevAddr);
                            Assert.AreEqual(long.MinValue, loadedBlock.NextAddr);
                        }
                        else if (emptyIdx == emptyAddrs.Count - 1)
                        {
                            Assert.AreEqual(long.MinValue, loadedBlock.PrevAddr);
                            Assert.AreEqual(emptyAddrs[emptyIdx - 1], loadedBlock.NextAddr);
                        }
                        else
                        {
                            Assert.AreEqual(emptyAddrs[emptyIdx + 1], loadedBlock.PrevAddr);
                            Assert.AreEqual(emptyAddrs[emptyIdx - 1], loadedBlock.NextAddr);
                        }
                    }
                    else
                    {
                        var block = blocks[i];
                        var loadedBlock = (DataBlock<WritableInt, Patient>)factory.ReadBlock(controlBlockSize + blockSize * i);
                        Utils.AssertDataBlocks(block, loadedBlock);
                    }
                }
            }
            File.Delete(testFile);
        }

        [TestMethod]
        public void FillEmptyBlocksTest()
        {
            var testFile = $"{DateTime.Now.Ticks}.bin";
            var random = new Random(1);
            var numBlocks = 10;
            var controlBlockSize = 18;
            var blockSize = Utils.RandomDataBlock(random).ByteSize;
            // write blocks
            var blocks = new List<DataBlock<WritableInt, Patient>>(numBlocks);
            using (var factory = new BlockFactory<WritableInt, Patient>(Utils.RandomDataBlockSize, testFile))
            {
                for (var i = 0; i < numBlocks; i++)
                {
                    var block = Utils.RandomDataBlock(random);
                    block.Address = factory.GetFreeAddress();
                    factory.WriteBlock(block);
                    blocks.Add(block);
                }
            }
            // remove all odd blocks
            var removed = new List<DataBlock<WritableInt, Patient>>();
            using (var factory = new BlockFactory<WritableInt, Patient>(Utils.RandomDataBlockSize, testFile))
            {
                for (var i = 0; i < numBlocks; i = i + 2)
                {
                    var block = blocks[i];
                    var loadedBlock = (DataBlock<WritableInt, Patient>)factory.ReadBlock(controlBlockSize + blockSize * i);
                    Utils.AssertDataBlocks(block, loadedBlock);
                    factory.RemoveBlock(loadedBlock);
                    removed.Add(loadedBlock);
                }
            }
            // fill empty blocks
            removed.Reverse();
            using (var factory = new BlockFactory<WritableInt, Patient>(Utils.RandomDataBlockSize, testFile))
            {
                foreach (var block in removed)
                {
                    block.Address = factory.GetFreeAddress();
                    factory.WriteBlock(block);
                }
            }
            // test if empty blocks are refilled
            using (var factory = new BlockFactory<WritableInt, Patient>(Utils.RandomDataBlockSize, testFile))
            {
                for (var i = 0; i < numBlocks; i++)
                {
                    var block = blocks[i];
                    var loadedBlock = (DataBlock<WritableInt, Patient>)factory.ReadBlock(controlBlockSize + blockSize * i);
                    Utils.AssertDataBlocks(block, loadedBlock);
                }
            }
            Assert.AreEqual(controlBlockSize + blockSize * numBlocks, new FileInfo(testFile).Length);
            File.Delete(testFile);
        }

        [TestMethod]
        public void CutFileTest()
        {
            var testFile = $"{DateTime.Now.Ticks}.bin";
            var random = new Random(1);
            var numBlocks = 10;
            var controlBlockSize = 18;
            var blockSize = Utils.RandomDataBlock(random).ByteSize;
            // write blocks
            var blocks = new List<DataBlock<WritableInt, Patient>>(numBlocks);
            using (var factory = new BlockFactory<WritableInt, Patient>(Utils.RandomDataBlockSize, testFile))
            {
                for (var i = 0; i < numBlocks; i++)
                {
                    var block = Utils.RandomDataBlock(random);
                    block.Address = factory.GetFreeAddress();
                    factory.WriteBlock(block);
                    blocks.Add(block);
                }
            }
            // cut end of file
            var blocksToRemove = 5;
            var fileLength = new FileInfo(testFile).Length;
            using (var factory = new BlockFactory<WritableInt, Patient>(Utils.RandomDataBlockSize, testFile))
            {
                for (var i = numBlocks - blocksToRemove; i < numBlocks - 1; i++)
                {
                    var block = blocks[i];
                    var loadedBlock = (DataBlock<WritableInt, Patient>)factory.ReadBlock(controlBlockSize + blockSize * i);
                    Utils.AssertDataBlocks(block, loadedBlock);
                    factory.RemoveBlock(loadedBlock);
                    Assert.AreEqual(fileLength, new FileInfo(testFile).Length);
                }
                var endBlock = (DataBlock<WritableInt, Patient>)factory.ReadBlock(controlBlockSize + blockSize * (numBlocks - 1));
                Utils.AssertDataBlocks(blocks.Last(), endBlock);
                factory.RemoveBlock(endBlock);
                Assert.AreEqual(controlBlockSize + blockSize * (numBlocks - blocksToRemove), new FileInfo(testFile).Length);
            }
            File.Delete(testFile);
        }

        [TestMethod]
        public void CutWholeFileTest()
        {
            var testFile = $"{DateTime.Now.Ticks}.bin";
            var random = new Random(1);
            var numBlocks = 10;
            var controlBlockSize = 18;
            var blockSize = Utils.RandomDataBlock(random).ByteSize;
            // write blocks
            var blocks = new List<DataBlock<WritableInt, Patient>>(numBlocks);
            using (var factory = new BlockFactory<WritableInt, Patient>(Utils.RandomDataBlockSize, testFile))
            {
                for (var i = 0; i < numBlocks; i++)
                {
                    var block = Utils.RandomDataBlock(random);
                    block.Address = factory.GetFreeAddress();
                    factory.WriteBlock(block);
                    blocks.Add(block);
                }
            }
            // cut whole file
            var fileLength = new FileInfo(testFile).Length;
            using (var factory = new BlockFactory<WritableInt, Patient>(Utils.RandomDataBlockSize, testFile))
            {
                for (var i = 0; i < numBlocks - 1; i++)
                {
                    var block = blocks[i];
                    var loadedBlock = (DataBlock<WritableInt, Patient>)factory.ReadBlock(controlBlockSize + blockSize * i);
                    Utils.AssertDataBlocks(block, loadedBlock);
                    factory.RemoveBlock(loadedBlock);
                    Assert.AreEqual(fileLength, new FileInfo(testFile).Length);
                }
                // remove last block
                var endBlock = (DataBlock<WritableInt, Patient>)factory.ReadBlock(controlBlockSize + blockSize * (numBlocks - 1));
                Utils.AssertDataBlocks(blocks.Last(), endBlock);
                factory.RemoveBlock(endBlock);
                Assert.AreEqual(controlBlockSize, new FileInfo(testFile).Length);
                // test if it will append blocks correctly, if NextFreeAddress is set to end of file
                for (var i = 0; i < numBlocks; i++)
                {
                    blocks[i].Address = factory.GetFreeAddress();
                    factory.WriteBlock(blocks[i]);
                }
                Assert.AreEqual(fileLength, new FileInfo(testFile).Length);
                // check reinserted blocks
                for (var i = 0; i < numBlocks - 1; i++)
                {
                    var block = blocks[i];
                    var loadedBlock = (DataBlock<WritableInt, Patient>)factory.ReadBlock(controlBlockSize + blockSize * i);
                    Utils.AssertDataBlocks(block, loadedBlock);
                }
            }
            File.Delete(testFile);
        }
    }
}
