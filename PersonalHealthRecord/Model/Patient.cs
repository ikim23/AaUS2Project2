using System;
using BPlusTree.Writables;

namespace PersonalHealthRecord.Model
{
    public class Patient : IWritable
    {
        public int ByteSize => ByteUtils.ByteSize(_firstName, _lastName, _birthday, _cardId, Hospitalizations);
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
        public DateTime Birthday
        {
            get => _birthday.Value;
            set => _birthday.Value = value;
        }
        public int CardId
        {
            get => _cardId.Value;
            set => _cardId.Value = value;
        }
        public WritableCollection<Hospitalization> Hospitalizations { get; }
        private readonly WritableString _firstName = new WritableString(25);
        private readonly WritableString _lastName = new WritableString(25);
        private readonly WritableDateTime _birthday = new WritableDateTime();
        private readonly WritableInt _cardId = new WritableInt();

        public Patient()
        {
            Hospitalizations = new WritableCollection<Hospitalization>(100);
        }

        public byte[] GetBytes() => ByteUtils.Join(_firstName, _lastName, _birthday, _cardId, Hospitalizations);

        public void FromBytes(byte[] bytes, int index = 0) => ByteUtils.FromBytes(bytes, index, _firstName, _lastName, _birthday, _cardId, Hospitalizations);

        public override string ToString() => $"FirstName: {_firstName} LastName: {_lastName} Birthday: {_birthday} CardId: {_cardId}";
    }
}
