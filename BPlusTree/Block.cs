using System;

namespace BPlusTree
{
    internal class Block<T> : IWritable where T : IWritable, new()
    {
        public int ByteSize => _records.Length * new T().ByteSize + sizeof(int);
        private readonly T[] _records;
        private int _recordsLength;
        
        public Block(int numRecords)
        {
            _records = new T[numRecords];
        }

        public T Get(int index) => _records[index];

        public void Add(T value)
        {
            _records[_recordsLength] = value;
            _recordsLength++;
        }

        public T Delete(int index)
        {
            var record = _records[index];
            for (var i = index + 1; i < _records.Length; i++)
            {
                _records[i - 1] = _records[i];
            }
            _recordsLength--;
            return record;
        }

        public byte[] GetBytes()
        {
            var bytes = new byte[ByteSize];
            var recordsLength = BitConverter.GetBytes(_recordsLength);
            Array.Copy(recordsLength, 0, bytes, 0, recordsLength.Length);
            var destIdx = recordsLength.Length;
            for (var i = 0; i < _recordsLength; i++)
            {
                var record = _records[i];
                Array.Copy(record.GetBytes(), 0, bytes, destIdx, record.ByteSize);
                destIdx += record.ByteSize;
            }
            return bytes;
        }

        public void FromBytes(byte[] bytes, int index = 0)
        {
            var srcIdx = index;
            _recordsLength = BitConverter.ToInt32(bytes, srcIdx);
            srcIdx += sizeof(int);
            for (var i = 0; i < _recordsLength; i++)
            {
                var record = _records[i];
                record.FromBytes(bytes, srcIdx);
                srcIdx += record.ByteSize;
            }
        }
    }
}
