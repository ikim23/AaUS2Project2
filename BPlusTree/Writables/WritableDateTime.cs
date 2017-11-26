using System;

namespace BPlusTree.Writables
{
    public class WritableDateTime : IComparable<WritableDateTime>, IWritable
    {
        public int ByteSize => sizeof(long);
        public DateTime Value;

        public WritableDateTime() : this(DateTime.MinValue)
        {
        }

        public WritableDateTime(DateTime value)
        {
            Value = value;
        }

        public byte[] GetBytes() => BitConverter.GetBytes(Value.Ticks);

        public void FromBytes(byte[] bytes, int index = 0)
        {
            var ticks = BitConverter.ToInt64(bytes, index);
            Value = new DateTime(ticks);
        }

        public int CompareTo(WritableDateTime other) => Value.CompareTo(other.Value);

        public override string ToString() => $"{Value:dd.MM.yyyy}";
    }
}
