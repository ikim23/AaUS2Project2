using System;
using BPlusTree.DataStructures;
using BPlusTree.Writables;

namespace BPlusTree.Blocks
{
    public class IndexBlock<TK> : IBlock where TK : IComparable<TK>, IWritable, new()
    {
        public static char Type => 'I';

        public long Address { get; set; }
        public int ByteSize { get; }
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

        public bool IsFull() => _keys.IsFull();

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
            var index = _keys.FindInsertionIndex(key);
            var k = _keys.Items[index];
            if (k != null && key.CompareTo(k) == 0)
                return _children[index + 1].Value;
            return _children[index].Value;
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
    }
}
