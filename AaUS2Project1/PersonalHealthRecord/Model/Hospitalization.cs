using System;
using System.Runtime.Serialization;

namespace PersonalHealthRecord.Model
{
    internal class Hospitalization
    {
        public string Hospital { get; set; }
        public int PersonalNumber
        {
            get => Patient?.PersonalNumber ?? _personalNumber;
            set => _personalNumber = value;
        }
        public string StartDate
        {
            get => DateFormatter.ToString(Start);
            set => Start = DateFormatter.Parse(value);
        }
        public string EndDate
        {
            get => DateFormatter.ToString(End);
            set => End = DateFormatter.ParseOrNull(value);
        }
        public string Diagnosis { get; set; }
        [IgnoreDataMember]
        public DateTime Start { get; set; }
        [IgnoreDataMember]
        public DateTime? End { get; set; }
        [IgnoreDataMember]
        public Patient Patient { get; set; }
        [IgnoreDataMember]
        public bool IsDone => End != null;

        private int _personalNumber;

        public string[] ToArray()
        {
            return new[]
            {
                Hospital,
                Start.ToString(),
                End?.ToString(),
                Diagnosis
            };
        }

        public string[] ToArrayWithPatient()
        {
            return new[]
            {
                Patient.Name,
                Patient.Surname,
                Patient.PersonalNumber.ToString(),
                Hospital,
                Start.ToString(),
                End?.ToString(),
                Diagnosis
            };
        }

        public override string ToString() => $"{Hospital}, {Patient.FullName}, {Start} - {End}";
    }
}
