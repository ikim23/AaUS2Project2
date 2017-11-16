using System;
using System.Linq;
using BPlusTree;
using BPlusTree.DataStructures;
using BPlusTree.Writables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataStructuresUnitTest
{
    [TestClass]
    public class SortedArrayUnitTest
    {
        [TestMethod]
        public void InsertTest()
        {
            const int arraySize = 1_000;
            for (var seed = 0; seed < 1_000; seed++)
            {
                var sortedArray = new SortedArray<WritableInt, WritableInt>(arraySize);
                var rand = new Random(seed);
                var expected = Enumerable.Range(0, arraySize).ToArray();
                var items = expected.OrderBy(i => rand.Next());
                foreach (var item in items)
                    sortedArray.Insert(new WritableInt(item), new WritableInt(item));
                var returned = sortedArray.Select(w => w.Value).ToArray();
                CollectionAssert.AreEqual(expected, returned, $@"
                Expected: {expected.Print()}
                Returned: {returned.Print()}");
            }
        }

        [TestMethod]
        public void SerializationTest()
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
                    Assert.AreEqual(serialized.FirstName, deserialized.FirstName);
                    Assert.AreEqual(serialized.LastName, deserialized.LastName);
                    Assert.AreEqual(serialized.Birthday, deserialized.Birthday);
                    Assert.AreEqual(serialized.CardId, deserialized.CardId);
                    var serializedHospitalizations = serialized.Hospitalizations.ToArray();
                    var deserializedHospitalizations = deserialized.Hospitalizations.ToArray();
                    Assert.AreEqual(serializedHospitalizations.Length, deserializedHospitalizations.Length);
                    for (var i = 0; i < serializedHospitalizations.Length; i++)
                    {
                        var serdHosp = serializedHospitalizations[i];
                        var desHosp = deserializedHospitalizations[i];
                        Assert.AreEqual(serdHosp.Start, desHosp.Start);
                        Assert.AreEqual(serdHosp.End, desHosp.End);
                        Assert.AreEqual(serdHosp.Diagnosis, desHosp.Diagnosis);
                    }
                }
            }
        }
    }
}
