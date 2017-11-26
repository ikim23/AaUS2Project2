using System;
using BPlusTree.DataStructures;
using BPlusTree.Writables;

namespace BPlusTree.Blocks
{
    internal class IndexBlock<TK> : IBlock where TK : IComparable<TK>, IWritable, new()
    {
        public static char Type => 'I';

        public long Address { get; set; }
        public int ByteSize { get; }
        private SortedSet<TK> _keys;
        private WritableLong[] _children;
        private WritableLong _parent;

        public IndexBlock(int byteSize, long parent)
        {
            ByteSize = byteSize;
            var size = CalculateSetSize(byteSize);
            _keys = new SortedSet<TK>(size);
            _children = new WritableLong[size + 1];
            _parent = new WritableLong(parent);
        }



        private int CalculateSetSize(int byteSize)
        {
            var longSize = new WritableLong().ByteSize;
            var bytes = byteSize - longSize * 2;
            return bytes / (new TK().ByteSize + longSize);
        }

        public byte[] GetBytes()
        {
            throw new NotImplementedException();
        }

        public void FromBytes(byte[] bytes, int index = 0)
        {
            throw new NotImplementedException();
        }
    }
}
