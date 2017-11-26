using System;
using System.Collections.Generic;
using System.Linq;
using BPlusTree.Writables;

namespace BPlusTree.DataStructures
{
    public class SortedSet<TK> : IWritable where TK : IComparable<TK>, IWritable, new()
    {
        public int ByteSize => sizeof(int) + MaxSize * (_items[0] != null ? _items[0] : new TK()).ByteSize;
        public int MaxSize { get; }
        public int Count { get; internal set; }
        public TK this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
                return _items[index];
            }
        }
        private readonly TK[] _items;

        public SortedSet(int size)
        {
            MaxSize = size;
            _items = new TK[size];
        }

        public void Insert(TK key)
        {
            if (Count == MaxSize) throw new ArgumentException("List is already full");
            var index = IndexOf(key);
            if (AreEqual(key, index)) throw new ArgumentException("Item with same key already exists");
            Array.Copy(_items, index, _items, index + 1, Count - index);
            _items[index] = key;
            Count++;
        }

        public void Remove(TK key)
        {
            var index = IndexOf(key);
            if (!AreEqual(key, index)) throw new KeyNotFoundException();
            Array.Copy(_items, index + 1, _items, index, Count - (index + 1));
            Count--;
        }

        public int IndexOf(TK key)
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

        public bool AreEqual(TK key, int itemOnIndex)
        {
            var item = _items[itemOnIndex];
            return item != null && key.CompareTo(item) == 0;
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
                var item = _items[i] != null ? _items[0] : new TK();
                item.FromBytes(bytes, srcIdx);
                _items[i] = item;
                srcIdx += item.ByteSize;
            }
        }
        
        public override string ToString() => $"\n{string.Join(",\n", _items.Take(Count))}";
    }
}
