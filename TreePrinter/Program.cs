using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPlusTree;
using BPlusTree.DataStructures;
using BPlusTree.Writables;

namespace TreePrinter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No input...");
            }
            else
            {
                var factory = new BlockFactory<WritableInt, WritableInt>(5, args[0]);
                factory.Print();
            }
            Console.ReadKey();
        }
    }
}
