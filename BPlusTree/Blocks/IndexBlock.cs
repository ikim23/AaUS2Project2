using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using BPlusTree.DataStructures;
using BPlusTree.Writables;

namespace BPlusTree.Blocks
{
    public class IndexBlock<TK> : IBlock where TK : IComparable<TK>, IWritable, new()
    {
        public static char Type => 'I';

        public long Address { get; set; }
        public int ByteSize { get; }
        public int FillFactor => Math.Max((int)(Math.Ceiling((double)_keys.MaxSize / 2) - 1), 1);
        private readonly WritableChar _type = new WritableChar(Type);
        private SortedIndex<TK> _keys;
        private readonly WritableArray<WritableLong> _children;

        public IndexBlock(int byteSize)
        {
            ByteSize = byteSize;
            var size = CalculateSize(byteSize);
            _keys = new SortedIndex<TK>(size);
            _children = new WritableArray<WritableLong>(size + 1);
        }

        public IndexBlock(int byteSize, IEnumerable<TK> items)
        {
            ByteSize = byteSize;
            var size = CalculateSize(byteSize);
            _keys = new SortedIndex<TK>(size, items);
            _children = new WritableArray<WritableLong>(size + 1);
        }

        public TK MinKey() => _keys.Min;

        public bool IsFull() => _keys.IsFull();

        public bool IsUnderFlow() => _keys.Count < FillFactor;

        public bool CanBorrow() => _keys.Count > FillFactor;

        public void Insert(TK key, long leftChild, long rightChild)
        {
            var index = _keys.FindInsertionIndex(key);
            _keys.Insert(key);
            var dstIdx = index + 1;
            Array.Copy(_children.Value, index, _children.Value, dstIdx, _children.Value.Length - dstIdx);
            _children[index] = new WritableLong(leftChild);
            _children[dstIdx] = new WritableLong(rightChild);
        }

        public IndexBlock<TK> Split(TK key, out TK middle, long leftChild, long rightChild)
        {
            var index = _keys.FindInsertionIndex(key);
            var dstIdx = index + 1;

            var splitArray = new WritableLong[_children.Value.Length + 1];
            Array.Copy(_children.Value, 0, splitArray, 0, _children.Value.Length);
            Array.Copy(_children.Value, index, splitArray, dstIdx, splitArray.Length - dstIdx);
            splitArray[index] = new WritableLong(leftChild);
            splitArray[dstIdx] = new WritableLong(rightChild);

            var rightKeys = _keys.Split(key, out middle);
            var rightBlock = new IndexBlock<TK>(ByteSize);
            rightBlock._keys = rightKeys;

            // split children addresses on index of middle element
            _children.Value = new WritableLong[_children.Value.Length];
            Array.Copy(splitArray, 0, _children.Value, 0, _keys.Count + 1);
            Array.Copy(splitArray, _keys.Count + 1, rightBlock._children.Value, 0, splitArray.Length - (_keys.Count + 1));

            return rightBlock;
        }

        public long MinAddress() => _children[0].Value;

        public long GetChildAddress(TK key)
        {
            var index = _keys.FindInsertionIndex(key);
            return _children[index].Value;
        }

        public long Find(TK key)
        {
            var index = ChildIndex(key);
            return _children[index].Value;
        }

        public bool Contains(TK key) => _keys.Contains(key);

        public int KeyIndex(TK key) => _keys.FindInsertionIndex(key);

        public int ChildIndex(TK key)
        {
            var index = _keys.FindInsertionIndex(key);
            var k = _keys.Items[index];
            if (k != null && key.CompareTo(k) == 0)
                return index + 1;
            return index;
        }

        public long GetChildAddress(int index)
        {
            if (index < 0 || index > _keys.Count) return long.MinValue;
            return _children[index].Value;
        }

        public void SetParentKey(int index, TK key)
        {
            if (index < 0 || index >= _keys.Count) throw new IndexOutOfRangeException();
            _keys.Items[index] = key;
        }

        public void MergeRemove(int index, long mergedChildAddr)
        { // children merged to left
            _keys.Remove(index);
            Array.Copy(_children.Value, index + 1, _children.Value, index, _children.Value.Length - (index + 1));
            _children.Value[index] = new WritableLong(mergedChildAddr);
        }

        public void ShiftMinFromRight(IndexBlock<TK> parent, int indexBlockIndex, IndexBlock<TK> right)
        {
            var rightMin = right._keys.RemoveMin();
            var parentMin = parent._keys.Update(indexBlockIndex, rightMin);
            _keys.AddToEnd(parentMin);
            _children[_keys.Count] = right._children[0];
            Array.Copy(right._children.Value, 1, right._children.Value, 0, right._children.Value.Length - 1);
        }

        public void ShiftMaxFromLeft(IndexBlock<TK> parent, int indexBlockIndex, IndexBlock<TK> left)
        {
            var maxChildAddress = left._children[left._keys.Count];
            var leftMax = left._keys.RemoveMax();
            var parentMax = parent._keys.Update(indexBlockIndex, leftMax);
            _keys.AddToStart(parentMax);
            Array.Copy(_children.Value, 0, _children.Value, 1, _children.Value.Length - 1);
            _children[0] = maxChildAddress;
        }

        public void Merge(IndexBlock<TK> parent, int parentIndex, IndexBlock<TK> left)
        {
            var middleKey = parent._keys.RemoveMin(); // joining key
            _keys.AddToEnd(middleKey);
            var dstIdx = _keys.Count;
            Array.Copy(left._keys.Items, 0, _keys.Items, _keys.Count, left._keys.Count);
            _keys.Count += left._keys.Count;
            Array.Copy(left._children.Value, 0, _children.Value, dstIdx, left._keys.Count + 1);
        }

        private int CalculateSize(int byteSize)
        {
            var longSize = new WritableLong().ByteSize;
            // subtract:
            // - Type
            // - extra address for last children (_children)
            // - 2x Count for WritableArray (_keys, _children)
            var bytes = byteSize - (_type.ByteSize + longSize + sizeof(int) * 2);
            return bytes / (new TK().ByteSize + longSize);
        }

        public byte[] GetBytes() => ByteUtils.Join(ByteSize, _type, _keys, _children);

        public void FromBytes(byte[] bytes, int index = 0) => ByteUtils.FromBytes(bytes, index + _type.ByteSize, _keys, _children);

        public override string ToString() => $"Type: {Type} Addr: {Address}\nKeys: {_keys.Count}";

        public Grid CreateGrid()
        {
            var grid = new Grid();
            var colLeft = new ColumnDefinition();
            var colRight = new ColumnDefinition();
            grid.ColumnDefinitions.Add(colLeft);
            grid.ColumnDefinitions.Add(colRight);
            var colorSwaper = UiUtils.GetColor().GetEnumerator();
            colorSwaper.MoveNext();
            var rowIndex = 0;
            UiUtils.AddGridRow(grid, rowIndex++, "Type:", Type, colorSwaper.Current);
            UiUtils.AddGridRow(grid, rowIndex++, "ByteSize:", ByteSize, colorSwaper.Current);
            UiUtils.AddGridRow(grid, rowIndex++, "Address:", Address, colorSwaper.Current);
            UiUtils.AddGridRow(grid, rowIndex++, "Keys:", null, colorSwaper.Current);
            var keyIndex = 0;
            foreach (var key in _keys)
            {
                var keyType = key.GetType();
                var keyProps = keyType.GetProperties();
                colorSwaper.MoveNext();
                UiUtils.AddGridRow(grid, rowIndex++, $"Key {keyIndex++}:", null, Colors.Transparent);
                foreach (var prop in keyProps)
                {
                    var value = prop.GetValue(key);
                    UiUtils.AddGridRow(grid, rowIndex++, prop.Name, value, colorSwaper.Current, 25);
                }
            }
            colorSwaper.MoveNext();
            UiUtils.AddGridRow(grid, rowIndex++, "Children Addresses:", null, Colors.Transparent);
            for (var childIndex = 0; childIndex < _keys.Count + 1; childIndex++)
            {
                colorSwaper.MoveNext();
                UiUtils.AddGridRow(grid, rowIndex++, $"Child {childIndex++}:", _children[childIndex].Value, colorSwaper.Current, 25);
            }
            colorSwaper.Dispose();
            return grid;
        }
    }
}
