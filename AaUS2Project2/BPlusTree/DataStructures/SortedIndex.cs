using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BPlusTree.Writables;

namespace BPlusTree.DataStructures
{
    public class SortedIndex<TK> : IEnumerable<TK>, IWritable where TK : IComparable<TK>, IWritable, new()
    {
        public int ByteSize => sizeof(int) + MaxSize * (Items[0] != null ? Items[0] : new TK()).ByteSize;
        public int MaxSize { get; }
        public int Count { get; internal set; }
        public TK Min => Items[0];
        public readonly TK[] Items;

        public SortedIndex(int size)
        {
            MaxSize = size;
            Items = new TK[size + 1];
        }

        public SortedIndex(int size, IEnumerable<TK> items) : this(size)
        {
            foreach (var item in items) Insert(item);
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
            if (!AreEqual(key, index)) throw new KeyNotFoundException($"Key {key} not found");
            return Items[index];
        }

        public SortedIndex<TK> Split(TK key, out TK middle)
        {
            if (!IsFull()) throw new ArgumentException($"Only {nameof(SortedIndex<TK>)} with full capacity can be splitted");
            InsertWithoutCountCheck(key);
            var midIndex = Count / 2;
            middle = Items[midIndex];
            // skip items after middle
            Count = midIndex;
            // copy items after middle to right split
            var srcIdx = midIndex + 1;
            var rightSplit = new SortedIndex<TK>(MaxSize);
            rightSplit.Count = Items.Length - srcIdx;
            Array.Copy(Items, srcIdx, rightSplit.Items, 0, rightSplit.Count);
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
            Array.Copy(Items, index, Items, index + 1, Count - index);
            Items[index] = key;
            Count++;
        }

        public TK Remove(TK key)
        {
            var index = FindInsertionIndex(key);
            if (!AreEqual(key, index)) throw new KeyNotFoundException();
            var value = Items[index];
            Array.Copy(Items, index + 1, Items, index, Count - (index + 1));
            Count--;
            return value;
        }

        public void Remove(int index)
        {
            Array.Copy(Items, index + 1, Items, index, Count - (index + 1));
            Count--;
        }

        public TK RemoveMax()
        {
            if (Count == 0) throw new InvalidOperationException("already empty");
            var max = Items[Count - 1];
            Count--;
            return max;
        }

        public TK ShiftMaxFromLeft(SortedIndex<TK> left)
        {
            var max = left.RemoveMax();
            Array.Copy(Items, 0, Items, 1, Count);
            Items[0] = max;
            Count++;
            return max; // return new middle key
        }

        public TK RemoveMin()
        {
            if (Count == 0) throw new InvalidOperationException("already empty");
            var min = Items[0];
            Array.Copy(Items, 1, Items, 0, Count);
            Count--;
            return min;
        }

        public TK RemoveAt(int index)
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
            var value = Items[index];
            Array.Copy(Items, index + 1, Items, index, Count - (index + 1));
            Count--;
            return value;
        }

        public TK Update(int index, TK newValue)
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
            var old = Items[index];
            Items[index] = newValue;
            return old;
        }

        public void AddToEnd(TK key)
        {
            Items[Count] = key;
            Count++;
        }

        public void AddToStart(TK key)
        {
            Array.Copy(Items, 0, Items, 1, Count);
            Items[0] = key;
            Count++;
        }

        public TK ShiftMinFromRight(SortedIndex<TK> right)
        {
            var min = right.RemoveMin();
            Items[Count] = min;
            Count++;
            return right.Min; // return new middle key
        }

        public void Merge(SortedIndex<TK> right)
        {
            Array.Copy(right.Items, 0, Items, Count, right.Count);
            Count += right.Count;
        }

        public int FindInsertionIndex(TK key)
        {
            var lowerBound = 0;
            var upperBound = Count - 1;
            while (lowerBound <= upperBound)
            {
                var index = lowerBound + (upperBound - lowerBound) / 2;
                var cmp = key.CompareTo(Items[index]);
                if (cmp < 0) upperBound = index - 1;
                else if (cmp > 0) lowerBound = index + 1;
                else return index;
            }
            return lowerBound;
        }

        private bool AreEqual(TK key, int itemOnIndex)
        {
            if (itemOnIndex >= MaxSize) return false;
            var item = Items[itemOnIndex];
            return item != null && key.CompareTo(item) == 0;
        }

        public byte[] GetBytes()
        {
            var bytes = new byte[ByteSize];
            var dstIdx = sizeof(int);
            Array.Copy(BitConverter.GetBytes(Count), 0, bytes, 0, dstIdx);
            for (var i = 0; i < Count; i++)
            {
                var item = Items[i];
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
                var item = Items[i] != null ? Items[0] : new TK();
                item.FromBytes(bytes, srcIdx);
                Items[i] = item;
                srcIdx += item.ByteSize;
            }
        }

        public IEnumerator<TK> GetEnumerator() => Items.Take(Count).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => string.Join(",", this);
    }
}
