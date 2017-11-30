using System;

namespace BPlusTree.Writables
{
    public class WritableArray<T> : IWritable where T : IWritable, new()
    {
        public int ByteSize { get; }
        public T this[int i]
        {
            get => Value[i];
            set => Value[i] = value;
        }
        public T[] Value;

        public WritableArray(int size)
        {
            ByteSize = sizeof(int) + size * new T().ByteSize;
            Value = new T[size];
        }

        public byte[] GetBytes()
        {
            var bytes = new byte[ByteSize];
            var dstIdx = 0;
            T item;
            for (var i = 0; i < Value.Length && (item = Value[i]) != null; i++)
            {
                Array.Copy(item.GetBytes(), 0, bytes, dstIdx, item.ByteSize);
                dstIdx += item.ByteSize;
            }
            return bytes;
        }

        public void FromBytes(byte[] bytes, int index = 0)
        {
            var srcIdx = index;
            for (var i = 0; i < Value.Length; i++)
            {
                var item = Value[i] != null ? Value[i] : new T();
                item.FromBytes(bytes, srcIdx);
                Value[i] = item;
                srcIdx += item.ByteSize;
            }
        }
    }
}
