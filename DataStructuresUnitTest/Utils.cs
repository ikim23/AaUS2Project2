using System;
using System.Collections.Generic;
using System.Linq;
using BPlusTree;
using BPlusTree.Writables;

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
    }
}
