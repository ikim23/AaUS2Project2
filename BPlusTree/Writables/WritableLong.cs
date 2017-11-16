using System;

namespace BPlusTree.Writables
{
    public class WritableLong : IComparable<WritableInt>, IWritable
    {
        public int ByteSize => sizeof(long);
        public long Value { get; set; }

        public WritableLong() : this(long.MinValue)
        {
        }

        public WritableLong(long value)
        {
            Value = value;
        }

        public int CompareTo(WritableInt other) => Value.CompareTo(other.Value);

        public byte[] GetBytes() => BitConverter.GetBytes(Value);

        public void FromBytes(byte[] bytes, int index = 0)
        {
            Value = BitConverter.ToInt64(bytes, index);
        }

        public override string ToString() => Value.ToString();
    }
}
