using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BPlusTree.Writables;

namespace BPlusTree.DataStructures
{
    public class SortedArray<TK, TV> : IEnumerable<TV>, IWritable where TK : IComparable<TK>, IWritable, new() where TV : IWritable, new()
    {
        public int ByteSize => MaxSize * new SortedArrayItem<TK, TV>().ByteSize + sizeof(int);
        public int MaxSize { get; }
        public int Count { get; internal set; }
        private readonly SortedArrayItem<TK, TV>[] _items;

        public SortedArray(int size)
        {
            MaxSize = size;
            _items = new SortedArrayItem<TK, TV>[size];
        }

        public void Insert(TK key, TV value)
        {
            if (Count == MaxSize) throw new ArgumentException("List is already full");
            var index = Bisection(key);
            Array.Copy(_items, index, _items, index + 1, Count - index);
            _items[index] = new SortedArrayItem<TK, TV>(key, value);
            Count++;
        }

        public TV Remove(TK key)
        {
            var index = FindIndex(key);
            var value = _items[index].Value;
            Array.Copy(_items, index + 1, _items, index, Count - (index + 1));
            Count--;
            return value;
        }

        public TV Find(TK key)
        {
            var index = FindIndex(key);
            return _items[index].Value;
        }

        private int FindIndex(TK key)
        {
            var lowerBound = 0;
            var upperBound = Count - 1;
            while (lowerBound <= upperBound)
            {
                var index = lowerBound + (upperBound - lowerBound) / 2;
                var cmp = key.CompareTo(_items[index].Key);
                if (cmp < 0) upperBound = index - 1;
                else if (cmp > 0) lowerBound = index + 1;
                else return index;
            }
            throw new KeyNotFoundException();
        }

        private int Bisection(TK key)
        {
            var lowerBound = 0;
            var upperBound = Count - 1;
            while (lowerBound <= upperBound)
            {
                var index = lowerBound + (upperBound - lowerBound) / 2;
                var cmp = key.CompareTo(_items[index].Key);
                if (cmp < 0) upperBound = index - 1;
                else if (cmp > 0) lowerBound = index + 1;
                else throw new ArgumentException("Item with same key already exists");
            }
            return lowerBound;
        }

        public byte[] GetBytes()
        {
            var bytes = new byte[ByteSize];
            Array.Copy(BitConverter.GetBytes(Count), 0, bytes, 0, sizeof(int));
            var dstIdx = sizeof(int);
            for (var i = 0; i < Count; i++)
            {
                var item = _items[i];
                Array.Copy(item.GetBytes(), 0, bytes, dstIdx, item.ByteSize);
                dstIdx += item.ByteSize;
            }
            return bytes;
        }

        public void FromBytes(byte[] bytes, int index = 0)
        {
            Count = BitConverter.ToInt32(bytes, index);
            var srcIdx = index + sizeof(int);
            for (var i = 0; i < Count; i++)
            {
                var item = _items[i] ?? new SortedArrayItem<TK, TV>();
                item.FromBytes(bytes, srcIdx);
                _items[i] = item;
                srcIdx += item.ByteSize;
            }
        }

        public TV[] ToArray() => _items.Select(i => i.Value).Take(Count).ToArray();

        public IEnumerator<TV> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return _items[i].Value;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal class SortedArrayItem<TK, TV> : IWritable where TK : IComparable<TK>, IWritable, new() where TV : IWritable, new()
    {
        public int ByteSize => ByteUtils.ByteSize(new TK(), new TV());
        public TK Key { get; internal set; }
        public TV Value { get; internal set; }

        public SortedArrayItem() : this(new TK(), new TV())
        {
        }

        public SortedArrayItem(TK key, TV value)
        {
            Key = key;
            Value = value;
        }

        public byte[] GetBytes() => ByteUtils.Join(Key, Value);

        public void FromBytes(byte[] bytes, int index = 0)
        {
            Key.FromBytes(bytes, index);
            Value.FromBytes(bytes, index + Key.ByteSize);
        }

        public override string ToString() => $"[{Key.ToString()}: {Value.ToString()}]";
    }
}
