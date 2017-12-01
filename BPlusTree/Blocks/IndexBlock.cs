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
        public long Parent
        {
            get => _parent.Value;
            set => _parent.Value = value;
        }
        private readonly WritableChar _type = new WritableChar(Type);
        private readonly WritableLong _parent = new WritableLong(long.MinValue);
        public SortedIndex<TK> _keys;
        public readonly WritableArray<WritableLong> _children;

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
            _children[index + 1] = new WritableLong(rightChild);
        }

        public IndexBlock<TK> Split(TK key, out TK middle, long leftChild, long rightChild)
        {
            var index = _keys.FindInsertionIndex(key);
            var dstIdx = index + 1;
            var children = new WritableLong[_children.Value.Length + 1];
            Array.Copy(_children.Value, 0, children, 0, _children.Value.Length); // copy all data
            Array.Copy(_children.Value, index, children, dstIdx, children.Length - dstIdx); // make room for new child
            children[index] = new WritableLong(leftChild);
            children[index + 1] = new WritableLong(rightChild);

            var rightKeys = _keys.Split(key, out middle);
            var rightBlock = new IndexBlock<TK>(ByteSize);
            rightBlock._keys = rightKeys;
            rightBlock.Parent = Parent; // parent will be overriden... remove

            // split children addresses, split on index of middle element
            _children.Value = new WritableLong[_children.Value.Length];
            Array.Copy(children, 0, _children.Value, 0, _keys.Count + 1);
            Array.Copy(children, _keys.Count + 1, rightBlock._children.Value, 0, children.Length - (_keys.Count + 1));

            return rightBlock;
        }

        public bool ContainsChild(long address)
        {
            WritableLong child;
            for (var i = 0; i < _children.Value.Length && (child = _children[i]) != null; i++)
                if (child.Value == address) return true;
            return false;
        }

        public long MinAddress()
        {
            return _children[0].Value;
        }

        public long GetChildAddress(TK key)
        {
            var index = _keys.FindInsertionIndex(key);
            //var k = _keys._items[index];
            //if (k != null && key.CompareTo(k) == 0)
            //{
            //    Console.WriteLine("baaa");
            //    return _children[index + 1].Value;
            //}
            return _children[index].Value;
        }

        public long Find(TK key)
        {
            var index = _keys.FindInsertionIndex(key);
            var k = _keys._items[index];
            if (k != null && key.CompareTo(k) == 0)
            {
                return _children[index + 1].Value;
            }
            return _children[index].Value;
        }

        private int CalculateSize(int byteSize)
        {
            var longSize = new WritableLong().ByteSize;
            // subtract type, parent and extra address
            var bytes = byteSize - (_type.ByteSize + longSize * 2);
            // number of (key, address) pairs
            return bytes / (new TK().ByteSize + longSize);
        }

        public byte[] GetBytes() => ByteUtils.Join(_type, _parent, _keys, _children);

        public void FromBytes(byte[] bytes, int index = 0) => ByteUtils.FromBytes(bytes, index + _type.ByteSize, _parent, _keys, _children);

        public override string ToString() => $"Type: {_type}\nByteSize: {ByteSize}\nParent: {_parent}\nAddress: {Address}\nKeys: {_keys.Count}";
    }
}
