using System;
using BPlusTree.DataStructures;
using BPlusTree.Writables;

namespace BPlusTree
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = typeof(Patient);
            Console.WriteLine(p.Name);
            Console.WriteLine(p.FullName);
            Console.WriteLine(p.AssemblyQualifiedName);
            Console.WriteLine(typeof(WritableInt).AssemblyQualifiedName);
            Console.WriteLine(typeof(WritableCollection<WritableInt>).AssemblyQualifiedName.Length);
            //Console.ReadKey();
            var type = Type.GetType(p.AssemblyQualifiedName);
        }
    }
}
