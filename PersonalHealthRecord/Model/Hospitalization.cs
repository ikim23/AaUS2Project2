using System;
using BPlusTree.Writables;

namespace PersonalHealthRecord.Model
{
    internal class Hospitalization : IWritable
    {
        public int ByteSize => ByteUtils.ByteSize(_start, _end, _diagnosis);
        public DateTime Start
        {
            get => _start.Value;
            set => _start.Value = value;
        }
        public DateTime? End
        {
            get => _end.Value == DateTime.MinValue ? (DateTime?) null : _end.Value;
            set => _end.Value = value ?? DateTime.MinValue;
        }
        public string Diagnosis
        {
            get => _diagnosis.Value;
            set => _diagnosis.Value = value;
        }
        private readonly WritableDateTime _start = new WritableDateTime();
        private readonly WritableDateTime _end = new WritableDateTime();
        private readonly WritableString _diagnosis = new WritableString(40);

        public byte[] GetBytes() => ByteUtils.Join(_start, _end, _diagnosis);

        public void FromBytes(byte[] bytes, int index = 0) => ByteUtils.FromBytes(bytes, index, _start, _end, _diagnosis);

        public override string ToString() => $"Start: {_start} End: {_end} Diagnosis: {_diagnosis}";
    }
}
