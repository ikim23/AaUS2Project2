using System;
using System.Collections.Generic;
using System.Linq;
using BPlusTree;
using BPlusTree.DataStructures;
using BPlusTree.Writables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataStructuresUnitTest
{
    internal static class Utils
    {
        public static string Print<T>(this IEnumerable<T> enumerable) => $"[{string.Join(", ", enumerable)}]";

        public static string RandomString(Random random, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var str = Enumerable.Repeat(chars, length).Select(ch => ch[random.Next(0, chars.Length)]).ToArray();
            return new string(str);
        }

        public static DateTime RandomDateTime(Random random) => new DateTime(random.Next(1900, 2000), random.Next(1, 13), random.Next(1, 25));

        public static Patient RandomPatient(Random random)
        {
            var patient = new Patient
            {
                FirstName = RandomString(random, random.Next(10, 25)),
                LastName = RandomString(random, random.Next(10, 25)),
                Birthday = RandomDateTime(random),
                CardId = random.Next()
            };
            var numHospitalizations = random.Next(10, 100);
            var date = RandomDateTime(random);
            for (var i = 0; i < numHospitalizations; i++)
            {
                patient.Hospitalizations.Insert(new WritableDateTime(date.AddDays(i)), new Hospitalization
                {
                    Start = RandomDateTime(random),
                    End = RandomDateTime(random),
                    Diagnosis = RandomString(random, random.Next(10, 40))
                });
            }
            return patient;
        }

        public static void AssertPatients(Patient p1, Patient p2)
        {
            Assert.AreEqual(p1.FirstName, p2.FirstName);
            Assert.AreEqual(p1.LastName, p2.LastName);
            Assert.AreEqual(p1.Birthday, p2.Birthday);
            Assert.AreEqual(p1.CardId, p2.CardId);
            var p1Hospitalizations = p1.Hospitalizations.ToArray();
            var p2Hospitalizations = p2.Hospitalizations.ToArray();
            Assert.AreEqual(p1Hospitalizations.Length, p2Hospitalizations.Length);
            for (var i = 0; i < p1Hospitalizations.Length; i++)
            {
                var p1Hospitalization = p1Hospitalizations[i];
                var p2Hospitalization = p2Hospitalizations[i];
                Assert.AreEqual(p1Hospitalization.Start, p2Hospitalization.Start);
                Assert.AreEqual(p1Hospitalization.End, p2Hospitalization.End);
                Assert.AreEqual(p1Hospitalization.Diagnosis, p2Hospitalization.Diagnosis);
            }
        }

        public static DataBlock<WritableInt, Patient> RandomDataBlock(Random random)
        {
            var block = new DataBlock<WritableInt, Patient>(10);
            var count = random.Next(1, block.MaxSize);
            for (var i = 0; i < count; i++)
                block.Insert(new WritableInt(i), RandomPatient(random));
            return block;
        }

        public static void AssertDataBlocks(DataBlock<WritableInt, Patient> b1, DataBlock<WritableInt, Patient> b2)
        {
            var b1KeyVals = b1.ToKeyValueArray();
            var b2KeyVals = b2.ToKeyValueArray();
            Assert.AreEqual(b1KeyVals.Length, b2KeyVals.Length);
            for (var i = 0; i < b1KeyVals.Length; i++)
            {
                var b1Tuple = b1KeyVals[i];
                var b2Tuple = b2KeyVals[i];
                Assert.AreEqual(b1Tuple.Item1.Value, b2Tuple.Item1.Value);
                AssertPatients(b1Tuple.Item2, b2Tuple.Item2);
            }
        }
    }
}
