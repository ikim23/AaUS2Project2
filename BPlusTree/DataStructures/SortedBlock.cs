using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BPlusTree.Writables;

namespace BPlusTree.DataStructures
{
    public class SortedBlock<TK, TV> : IEnumerable<TV>, IWritable where TK : IComparable<TK>, IWritable, new() where TV : IWritable, new()
    {
        public int ByteSize => _items.ByteSize;
        public int MaxSize => _items.MaxSize;
        public int Count => _items.Count;
        private SortedIndex<SortedBlockItem<TK, TV>> _items;

        public SortedBlock(int size)
        {
            _items = new SortedIndex<SortedBlockItem<TK, TV>>(size);
        }

        public bool IsFull() => _items.IsFull();

        public bool Contains(TK key)
        {
            var item = new SortedBlockItem<TK, TV> { Key = key };
            return _items.Contains(item);
        }

        public TV Find(TK key)
        {
            var itemToFind = new SortedBlockItem<TK, TV> { Key = key };
            var foundItem = _items.Find(itemToFind);
            return foundItem.Value;
        }

        public SortedBlock<TK, TV> Split(TK key, TV value, out TK middle)
        {
            var item = new SortedBlockItem<TK, TV>(key, value);
            var rightSplit = _items.Split(item, out var middleTuple);
            var rightBlock = new SortedBlock<TK, TV>(MaxSize);
            rightBlock._items = rightSplit;
            rightBlock.Insert(middleTuple.Key, middleTuple.Value);
            middle = middleTuple.Key;
            return rightBlock;
        }

        public void Insert(TK key, TV value)
        {
            var item = new SortedBlockItem<TK, TV>(key, value);
            _items.Insert(item);
        }

        public TV Remove(TK key)
        {
            var itemToRemove = new SortedBlockItem<TK, TV> { Key = key };
            var removedItem = _items.Remove(itemToRemove);
            return removedItem.Value;
        }

        public int FindInsertionIndex(TK key)
        {
            var item = new SortedBlockItem<TK, TV> { Key = key };
            return _items.FindInsertionIndex(item);
        }

        public byte[] GetBytes() => _items.GetBytes();

        public void FromBytes(byte[] bytes, int index = 0) => _items.FromBytes(bytes, index);

        public IEnumerator<TV> GetEnumerator() => _items.Select(i => i.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => string.Join(",", this);
    }

    internal class SortedBlockItem<TK, TV> : IComparable<SortedBlockItem<TK, TV>>, IWritable where TK : IComparable<TK>, IWritable, new() where TV : IWritable, new()
    {
        public int ByteSize => ByteUtils.ByteSize(Key, Value);
        public TK Key { get; internal set; }
        public TV Value { get; internal set; }

        public SortedBlockItem() : this(new TK(), new TV())
        {
        }

        public SortedBlockItem(TK key, TV value)
        {
            Key = key;
            Value = value;
        }

        public int CompareTo(SortedBlockItem<TK, TV> other) => Key.CompareTo(other.Key);

        public byte[] GetBytes() => ByteUtils.Join(Key, Value);

        public void FromBytes(byte[] bytes, int index = 0) => ByteUtils.FromBytes(bytes, index, Key, Value);

        public override string ToString() => Key.ToString();
    }
}
