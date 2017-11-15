using BPlusTree.Writables;

namespace BPlusTree.DataStructures
{
    internal class DataBlock<TK, TV> : IWritable where TK : IWritable, new() where TV : IWritable, new()
    {
        public int ByteSize => MaxSize * (new TK().ByteSize + new TV().ByteSize) + sizeof(int);
        public int MaxSize { get; }
        //private SortedArray<TK, TV> _records;
        //public DataBlock(int size)
        //{
        //    MaxSize = size;
        //    _records = new SortedArray<TK, TV>(size);
        //}

        public byte[] GetBytes()
        {
            var bytes = new byte[ByteSize];
            //var count = BitConverter.GetBytes(_records.Count);
            //Array.Copy(count, 0, bytes, 0, sizeof(int));
            //var destIdx = sizeof(int);
            //foreach (var record in _records)
            //{
            //    var key = record.Key;
            //    Array.Copy(key.GetBytes(), 0, bytes, destIdx, key.ByteSize);
            //    destIdx += key.ByteSize;
            //    var value = record.Value;
            //    Array.Copy(value.GetBytes(), 0, bytes, destIdx, value.ByteSize);
            //    destIdx += value.ByteSize;
            //}
            return bytes;
        }

        public void FromBytes(byte[] bytes, int index = 0)
        {
            //var srcIdx = index;
            //var count = BitConverter.ToInt32(bytes, srcIdx);
            //srcIdx += sizeof(int);
            //for (var i = 0; i < count; i++)
            //{
            //    var a = new TK().FromBytes();
            //}
        }
    }
}
