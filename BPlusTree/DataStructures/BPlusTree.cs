using System;
using BPlusTree.Writables;

namespace BPlusTree.DataStructures
{
    public class BPlusTree<TK, TV> where TK : IComparable<TK>, IWritable, new() where TV : IWritable, new()
    {
        private readonly BlockFactory<TK, TV> _factory;

        public BPlusTree(int blockSize, string file = "bptree.bin")
        {
            _factory = new BlockFactory<TK, TV>(blockSize, file);
        }
    }
}
