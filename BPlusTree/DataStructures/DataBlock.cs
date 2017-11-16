using System;
using BPlusTree.Writables;

namespace BPlusTree.DataStructures
{
    public class DataBlock<TK, TV> : IWritable where TK : IComparable<TK>, IWritable, new() where TV : IWritable, new()
    {
        public int ByteSize => sizeof(long) + _records.ByteSize;
        public int MaxSize => _records.MaxSize;
        public long NextBlock
        {
            get => _nextBlock.Value;
            set => _nextBlock.Value = value;
        }
        private readonly SortedArray<TK, TV> _records;
        private readonly WritableLong _nextBlock = new WritableLong();

        public DataBlock(int size)
        {
            _records = new SortedArray<TK, TV>(size);
        }

        public void Insert(TK key, TV value) => _records.Insert(key, value);

        public TV Remove(TK key) => _records.Remove(key);

        public TV Find(TK key) => _records.Find(key);

        public Tuple<TK, TV>[] ToKeyValueArray() => _records.ToKeyValueArray();

        public byte[] GetBytes() => ByteUtils.Join(_nextBlock, _records);

        public void FromBytes(byte[] bytes, int index = 0) => ByteUtils.FromBytes(bytes, index, _nextBlock, _records);
    }
}
