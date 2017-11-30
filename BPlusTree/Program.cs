using System;
using BPlusTree.DataStructures;
using BPlusTree.Writables;

namespace BPlusTree
{
    class Program
    {
        static void Main(string[] args)
        {
            var size = 5;
            SortedIndex<WritableInt> idx = new SortedIndex<WritableInt>(size);
            var i = 0;
            int insertIdx;
            for (; i < size; i++)
            {
                insertIdx = idx.FindInsertionIndex(new WritableInt(i));
                Console.WriteLine($"{i}: insertion idx - {insertIdx}");
                idx.Insert(new WritableInt(i));
            }
            insertIdx = idx.FindInsertionIndex(new WritableInt(i));
            Console.WriteLine($"{i}: insertion idx - {insertIdx}");
            for (var j = 0; j < size; j++)
            {
                insertIdx = idx.FindInsertionIndex(new WritableInt(j));
                Console.WriteLine($"{j}: insertion idx - {insertIdx}");
            }
        }
    }
}
