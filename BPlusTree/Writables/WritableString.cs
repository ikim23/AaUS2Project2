using System;
using System.Text;

namespace BPlusTree.Writables
{
    public class WritableString : IComparable<WritableString>, IWritable
    {
        public int ByteSize => sizeof(int) + MaxLength;
        public int MaxLength { get; }
        public string Value
        {
            get => _value;
            set
            {
                if (value.Length > MaxLength) throw new ArgumentException($"Value tool long. Expected {MaxLength} characters.");
                _value = value;
            }
        }
        private string _value = String.Empty;

        public WritableString()
        {
        }

        public WritableString(int maxLength)
        {
            MaxLength = maxLength;
        }

        public byte[] GetBytes()
        {
            var valueLength = BitConverter.GetBytes(Value.Length);
            var value = Encoding.UTF8.GetBytes(Value);
            var bytes = new byte[ByteSize];
            Array.Copy(valueLength, 0, bytes, 0, valueLength.Length);
            Array.Copy(value, 0, bytes, valueLength.Length, value.Length);
            return bytes;
        }

        public void FromBytes(byte[] bytes, int index = 0)
        {
            var valueLength = BitConverter.ToInt32(bytes, index);
            Value = Encoding.UTF8.GetString(bytes, index + sizeof(int), valueLength);
        }

        public int CompareTo(WritableString other) => Value.CompareTo(other.Value);

        public override string ToString() => Value;
    }
}