using System;

namespace BPlusTree.Writables
{
    public class WritableChar : IComparable<WritableChar>, IWritable
    {
        public int ByteSize => sizeof(char);
        public char Value { get; set; }

        public WritableChar() : this(default(char))
        {
        }

        public WritableChar(char value)
        {
            Value = value;
        }

        public byte[] GetBytes() => BitConverter.GetBytes(Value);

        public void FromBytes(byte[] bytes, int index = 0)
        {
            Value = BitConverter.ToChar(bytes, index);
        }

        public int CompareTo(WritableChar other) => Value.CompareTo(other.Value);

        public override string ToString() => Value.ToString();
    }
}
