using System;

namespace BPlusTree.Writables
{
    public class WritableInt : IComparable<WritableInt>, IWritable
    {
        public int ByteSize => sizeof(int);
        public int Value { get; set; }

        public WritableInt() : this(int.MinValue)
        {
        }

        public WritableInt(int value)
        {
            Value = value;
        }

        public int CompareTo(WritableInt other) => Value.CompareTo(other.Value);

        public byte[] GetBytes() => BitConverter.GetBytes(Value);

        public void FromBytes(byte[] bytes, int index = 0)
        {
            Value = BitConverter.ToInt32(bytes, index);
        }

        public override string ToString() => Value.ToString();
    }
}
