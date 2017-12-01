using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BPlusTree.Writables;

namespace BPlusTree.DataStructures
{
    public class SortedIndex<TK> : IEnumerable<TK>, IWritable where TK : IComparable<TK>, IWritable, new()
    {
        public int ByteSize => sizeof(int) + MaxSize * (_items[0] != null ? _items[0] : new TK()).ByteSize;
        public int MaxSize { get; }
        public int Count { get; internal set; }
        public TK[] _items; // TODO: private

        public SortedIndex(int size)
        {
            MaxSize = size;
            _items = new TK[size + 1];
        }

        public bool IsFull() => Count >= MaxSize;

        public bool Contains(TK key)
        {
            var index = FindInsertionIndex(key);
            return AreEqual(key, index);
        }

        public TK Find(TK key)
        {
            var index = FindInsertionIndex(key);
            if (!AreEqual(key, index)) throw new KeyNotFoundException($"key {key} not found");
            return _items[index];
        }

        public SortedIndex<TK> Split(TK key, out TK middle)
        {
            if (!IsFull()) throw new ArgumentException($"Only {nameof(SortedIndex<TK>)} with full capacity can be splitted");
            InsertWithoutCountCheck(key);
            var midIndex = Count / 2;
            middle = _items[midIndex];
            // skip items after middle
            Count = midIndex;

            // clear _items
            var items = new TK[_items.Length];
            Array.Copy(_items, 0, items, 0, items.Length);
            _items = new TK[_items.Length];
            Array.Copy(items, 0, _items, 0, items.Length - midIndex + 1); // set back

            // copy items after middle
            var srcIdx = midIndex + 1;
            var rightSplit = new SortedIndex<TK>(MaxSize);
            Array.Copy(items, srcIdx, rightSplit._items, 0, items.Length - srcIdx);
            rightSplit.Count = items.Length - srcIdx;
            return rightSplit;
        }

        public void Insert(TK key)
        {
            if (Count == MaxSize) throw new ArgumentException("Capacity already full");
            InsertWithoutCountCheck(key);
        }

        private void InsertWithoutCountCheck(TK key)
        {
            var index = FindInsertionIndex(key);
            if (AreEqual(key, index)) throw new ArgumentException($"Item {key} is already inserted");
            Array.Copy(_items, index, _items, index + 1, Count - index);
            _items[index] = key;
            Count++;
        }

        public TK Remove(TK key)
        {
            var index = FindInsertionIndex(key);
            if (!AreEqual(key, index)) throw new KeyNotFoundException();
            var value = _items[index];
            Array.Copy(_items, index + 1, _items, index, Count - (index + 1));
            Count--;
            return value;
        }

        public int FindInsertionIndex(TK key)
        {
            var lowerBound = 0;
            var upperBound = Count - 1;
            while (lowerBound <= upperBound)
            {
                var index = lowerBound + (upperBound - lowerBound) / 2;
                var cmp = key.CompareTo(_items[index]);
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
            return item != null && key.CompareTo(item) == 0;
        }

        public byte[] GetBytes()
        {
            var bytes = new byte[ByteSize];
            var dstIdx = sizeof(int);
            Array.Copy(BitConverter.GetBytes(Count), 0, bytes, 0, dstIdx);
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
                var item = _items[i] != null ? _items[0] : new TK();
                item.FromBytes(bytes, srcIdx);
                _items[i] = item;
                srcIdx += item.ByteSize;
            }
        }

        public TK[] ToArray() => _items.Take(Count).ToArray();

        public IEnumerator<TK> GetEnumerator() => _items.Take(Count).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => $"\n{string.Join(",\n", _items.Take(Count))}";
    }
}
