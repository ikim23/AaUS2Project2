﻿using System;
using BPlusTree.DataStructures;
using BPlusTree.Writables;

namespace BPlusTree.Blocks
{
    public class DataBlock<TK, TV> : IBlock where TK : IComparable<TK>, IWritable, new() where TV : IWritable, new()
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

        public Tuple<TK, TV>[] ToKeyValueArray() => _records.ToKeyValueArray();

        public byte[] GetBytes() => ByteUtils.Join(_type, _nextBlock, _records);

        public void FromBytes(byte[] bytes, int index = 0) => ByteUtils.FromBytes(bytes, index + _type.ByteSize, _nextBlock, _records);

        public override string ToString() => $"Type: {_type}\nByteSize: {ByteSize}\nAddress: {Address}\nNextBlock: {_nextBlock}\nRecords: {_records.Count}";
        //public override string ToString() => $"NextBlock: {_nextBlock}\nRecords: {_records}";
    }
}
