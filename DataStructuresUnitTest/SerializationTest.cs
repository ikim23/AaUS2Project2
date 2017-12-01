using System;
using BPlusTree;
using BPlusTree.Blocks;
using BPlusTree.Writables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataStructuresUnitTest
{
    [TestClass]
    public class SerializationTest
    {
        [TestMethod]
        public void SerializePatientTest()
        {
            for (var rep = 0; rep < 100; rep++)
            {
                for (var seed = 0; seed < 100; seed++)
                {
                    var rand = new Random(seed);
                    var serialized = Utils.RandomPatient(rand);
                    var bytes = serialized.GetBytes();
                    var deserialized = new Patient();
                    deserialized.FromBytes(bytes);
                    Utils.AssertPatients(serialized, deserialized);
                }
            }
        }

        [TestMethod]
        public void SerializeDataBlockTest()
        {
            for (var rep = 0; rep < 100; rep++)
            {
                for (var seed = 0; seed < 100; seed++)
                {
                    var rand = new Random(seed);
                    var serialized = Utils.RandomDataBlock(rand);
                    var bytes = serialized.GetBytes();
                    var deserialized = new DataBlock<WritableInt, Patient>(10);
                    deserialized.FromBytes(bytes);
                    Utils.AssertDataBlocks(serialized, deserialized);
                }
            }
        }

        [TestMethod]
        public void BlockSizeMatchTest()
        {
            for (var blockSize = 5; blockSize < 100; blockSize++)
            {
                var dataBlock = new DataBlock<WritableInt, WritableInt>(blockSize);
                var indexBlock = new IndexBlock<WritableInt>(dataBlock.ByteSize);

                Assert.AreEqual(dataBlock.ByteSize, indexBlock.ByteSize);
                var dataBytes = dataBlock.GetBytes().Length;
                var indexBytes = indexBlock.GetBytes().Length;
                Assert.AreEqual(dataBytes, indexBytes, $"BlockSize: {blockSize} Data: {dataBytes} Index: {indexBytes}");
            }
        }
    }
}
