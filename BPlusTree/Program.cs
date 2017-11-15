using System;
using System.Collections.Generic;
using System.Linq;
using BPlusTree.DataStructures;
using BPlusTree.Writables;

namespace BPlusTree
{
    class Program
    {
        static void Main(string[] args)
        {
            //InsertTest();
            SerializationTest();
            //SortedArraySerializationTest();
            //PatientSerializationTest();

            var i = new WritableInt(10);
            var bytes = i.GetBytes();
            i = new WritableInt();
            i.FromBytes(bytes);

            Console.ReadKey();
        }

        static void InsertTest()
        {
            const int size = 1000;
            var sortedArray = new SortedArray<WritableInt, WritableInt>(size);
            var rand = new Random(1);
            var expected = Enumerable.Range(0, size - 1).ToArray();
            var items = expected.OrderBy(i => rand.Next());
            foreach (var item in items)
            {
                Console.WriteLine(item);
                sortedArray.Insert(new WritableInt(item), new WritableInt(item));
            }
            var resultArray = sortedArray.ToArray().Select(i => i.Value).ToArray();
            var result = AreEqual(expected, resultArray);
            if (!result)
            {
                Console.WriteLine(string.Join(", ", expected));
                Console.WriteLine(string.Join(", ", resultArray));
            }
            Console.WriteLine($"InsertTest: {result}");
        }

        static bool AreEqual<T>(IEnumerable<T> e1, IEnumerable<T> e2) where T : IComparable<T>
        {
            var en1 = e1.GetEnumerator();
            var en2 = e2.GetEnumerator();
            var move1 = en1.MoveNext();
            var move2 = en2.MoveNext();
            while (move1 || move2)
            {
                if (en1.Current.CompareTo(en2.Current) != 0) return false;
                move1 = en1.MoveNext();
                move2 = en2.MoveNext();
            }
            return true;
        }

        static void SerializationTest()
        {
            var patient = new Patient
            {
                FirstName = "John",
                LastName = "Snow",
                Birthday = DateTime.Today,
                CardId = 101,
            };
            patient.Hospitalizations.Insert(new WritableDateTime(DateTime.Today), new Hospitalization
            {
                Start = DateTime.Today,
                End = DateTime.Today,
                Diagnosis = "Chocolate cake sesame snaps."
            });
            var bytes = patient.GetBytes();
            patient = new Patient();
            patient.FromBytes(bytes);
        }

        static void PatientSerializationTest()
        {
            var patient = new Patient
            {
                FirstName = "John",
                LastName = "Snow",
                Birthday = DateTime.Today,
                CardId = 101
            };
            var bytes = patient.GetBytes();
            patient = new Patient();
            patient.FromBytes(bytes);
        }

        static void SortedArraySerializationTest()
        {
            var size = 10;
            var sortedArray = new SortedArray<WritableInt, WritableInt>(size);
            for (var i = 0; i < size; i++)
                sortedArray.Insert(new WritableInt(i), new WritableInt(i));
            var bytes = sortedArray.GetBytes();
            sortedArray = new SortedArray<WritableInt, WritableInt>(size);
            sortedArray.FromBytes(bytes);
        }
    }
}
