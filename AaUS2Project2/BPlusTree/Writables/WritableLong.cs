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

        public byte[] GetBytes() => BitConverter.GetBytes(Value);

        public void FromBytes(byte[] bytes, int index = 0)
        {
            Value = BitConverter.ToInt64(bytes, index);
        }

        public int CompareTo(WritableInt other) => Value.CompareTo(other.Value);

        public override string ToString() => Value.ToString();
    }
}
