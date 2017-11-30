using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BPlusTree.Writables;

namespace BPlusTree.DataStructures
{
    public class SortedArray<TK, TV> : IEnumerable<TV>, IWritable where TK : IComparable<TK>, IWritable, new() where TV : IWritable, new()
    {
        public int ByteSize => sizeof(int) + MaxSize * (_items[0] ?? new SortedArrayItem<TK, TV>()).ByteSize;
        public int MaxSize { get; }
        public int Count { get; internal set; }
        private readonly SortedArrayItem<TK, TV>[] _items;

        public SortedArray(int size)
        {
            MaxSize = size;
            _items = new SortedArrayItem<TK, TV>[size + 1];
        }

        public TK Min()
        {
            if (Count == 0) throw new InvalidOperationException();
            return _items[0].Key;
        }

        public SortedArray<TK, TV> Split(TK key, TV value)
        {
            if (!IsFull()) throw new ArgumentException($"Only full {nameof(SortedArray<TK, TV>)} can be split");
            InsertNoMaxSizeCheck(key, value);
            var rightSplit = new SortedArray<TK, TV>(MaxSize);
            var middle = Count / 2;
            Array.Copy(_items, middle, rightSplit._items, 0, Count - middle);
            rightSplit.Count = Count - middle;
            Count = middle;
            return rightSplit;
        }

        public bool IsFull() => Count >= MaxSize;

        public void Insert(TK key, TV value)
        {
            if (Count == MaxSize) throw new ArgumentException("List is already full");
            InsertNoMaxSizeCheck(key, value);
        }

        private void InsertNoMaxSizeCheck(TK key, TV value)
        {
            var index = FindInsertionIndex(key);
            if (AreEqual(key, index)) throw new ArgumentException("Item with same key already exists");
            Array.Copy(_items, index, _items, index + 1, Count - index);
            _items[index] = new SortedArrayItem<TK, TV>(key, value);
            Count++;
        }

        public TV Remove(TK key)
        {
            var index = FindInsertionIndex(key);
            if (!AreEqual(key, index)) throw new KeyNotFoundException();
            var value = _items[index].Value;
            Array.Copy(_items, index + 1, _items, index, Count - (index + 1));
            Count--;
            return value;
        }

        public bool Contains(TK key)
        {
            var index = FindInsertionIndex(key);
            return AreEqual(key, index);
        }

        public TV Find(TK key)
        {
            var index = FindInsertionIndex(key);
            if (!AreEqual(key, index)) throw new KeyNotFoundException();
            return _items[index].Value;
        }

        public int FindInsertionIndex(TK key)
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
            return lowerBound;
        }

        private bool AreEqual(TK key, int itemOnIndex)
        {
            if (itemOnIndex >= MaxSize) return false;
            var item = _items[itemOnIndex];
            return item != null && key.CompareTo(item.Key) == 0;
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

        public Tuple<TK, TV>[] ToKeyValueArray() => _items.Select(i => new Tuple<TK, TV>(i.Key, i.Value)).Take(Count).ToArray();

        public IEnumerator<TV> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return _items[i].Value;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => $"\n{string.Join(",\n", _items.Take(Count))}";
    }

    internal class SortedArrayItem<TK, TV> : IWritable where TK : IComparable<TK>, IWritable, new() where TV : IWritable, new()
    {
        public int ByteSize => ByteUtils.ByteSize(Key, Value);
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

        public void FromBytes(byte[] bytes, int index = 0) => ByteUtils.FromBytes(bytes, index, Key, Value);

        public override string ToString() => StringUtils.PadArrayItem($"Key: {Key},\nValue: {StringUtils.Pad(Value.ToString())}");
    }
}
