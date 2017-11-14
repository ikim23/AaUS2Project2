using System;

namespace BPlusTree
{
    public class Patient : IWritable
    {
        public int ByteSize => _firstName.ByteSize + _lastName.ByteSize + sizeof(long) + sizeof(int);
        public string FirstName
        {
            get => _firstName.Value;
            set => _firstName.Value = value;
        }

        public string LastName
        {
            get => _lastName.Value;
            set => _lastName.Value = value;
        }
        public DateTime Birthday { get; set; }
        public int CardId { get; set; }
        private FixedString _firstName = new FixedString(25);
        private FixedString _lastName = new FixedString(25);

        public byte[] GetBytes()
        {
            var birthday = BitConverter.GetBytes(Birthday.Ticks);
            var cardId = BitConverter.GetBytes(CardId);
            var destIdx = 0;
            var bytes = new byte[ByteSize];
            Array.Copy(_firstName.GetBytes(), 0, bytes, destIdx, _firstName.ByteSize);
            destIdx += _firstName.ByteSize;
            Array.Copy(_lastName.GetBytes(), 0, bytes, destIdx, _lastName.ByteSize);
            destIdx += _lastName.ByteSize;
            Array.Copy(birthday, 0, bytes, destIdx, sizeof(long));
            destIdx += sizeof(long);
            Array.Copy(cardId, 0, bytes, destIdx, sizeof(int));
            return bytes;
        }

        public void FromBytes(byte[] bytes, int index = 0)
        {
            var srcIdx = index;
            _firstName.FromBytes(bytes, srcIdx);
            srcIdx += _firstName.ByteSize;
            _lastName.FromBytes(bytes, srcIdx);
            srcIdx += _lastName.ByteSize;
            Birthday = new DateTime(BitConverter.ToInt64(bytes, srcIdx));
            srcIdx += sizeof(long);
            CardId = BitConverter.ToInt32(bytes, srcIdx);
        }
    }
}
