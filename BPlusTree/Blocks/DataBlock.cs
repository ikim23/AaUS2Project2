using System;
using System.Collections;
using System.Collections.Generic;
using BPlusTree.DataStructures;
using BPlusTree.Writables;

namespace BPlusTree.Blocks
{
    public class DataBlock<TK, TV> : IBlock, IEnumerable<TV> where TK : IComparable<TK>, IWritable, new() where TV : IWritable, new()
    {
        public static char Type => 'D';

        public long Address { get; set; }
        public int ByteSize => ByteUtils.ByteSize(_type, _nextBlock, _records);
        public int MaxSize => _records.MaxSize;
        public long NextBlock
        {
            get => _nextBlock.Value;
            set => _nextBlock.Value = value;
        }
        private readonly WritableChar _type = new WritableChar(Type);
        private readonly WritableLong _nextBlock = new WritableLong(long.MinValue);
        private SortedBlock<TK, TV> _records;

        public DataBlock(int size)
        {
            Address = long.MinValue;
            _records = new SortedBlock<TK, TV>(size);
        }

        public DataBlock<TK, TV> Split(TK key, TV value, out TK middle)
        {
            var rightRecords = _records.Split(key, value, out middle);
            var rightBlock = new DataBlock<TK, TV>(MaxSize)
            {
                _records = rightRecords,
                NextBlock = NextBlock
            };
            return rightBlock;
        }

        public bool IsFull() => _records.IsFull();

        public void Insert(TK key, TV value) => _records.Insert(key, value);

        public TV Find(TK key) => _records.Find(key);

        public byte[] GetBytes() => ByteUtils.Join(_type, _nextBlock, _records);

        public void FromBytes(byte[] bytes, int index = 0) => ByteUtils.FromBytes(bytes, index + _type.ByteSize, _nextBlock, _records);

        public IEnumerator<TV> GetEnumerator() => _records.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => $"Type: {Type} Addr: {Address} Next: {NextBlock} Records: {_records.Count}";
    }
}
