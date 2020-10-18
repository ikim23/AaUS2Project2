using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PersonalHealthRecord.Model
{
    internal class Patient
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string BirthdayDate {
            get => DateFormatter.ToString(Birthday);
            set => Birthday = DateFormatter.Parse(value);
        }
        public int PersonalNumber { get; set; }
        public string AgencyCode { get; set; }
        [IgnoreDataMember]
        public DateTime Birthday { get; set; }
        [IgnoreDataMember]
        public string FullName => $"{Name} {Surname}";
        [IgnoreDataMember]
        public LinkedList<Hospitalization> Hospitalizations { get; internal set; }

        public Patient()
        {
            Hospitalizations = new LinkedList<Hospitalization>();
        }

        public Patient(string name, string surname, DateTime birthday, int personalNumber, string agencyCode) : this()
        {
            Name = name;
            Surname = surname;
            Birthday = birthday;
            PersonalNumber = personalNumber;
            AgencyCode = agencyCode;
        }

        public Hospitalization StartHospitalization(DateTime start, string diagnosis)
        {
            var lastHospitalization = Hospitalizations.Last?.Value;
            if (lastHospitalization != null && !lastHospitalization.IsDone) throw new Exception($"Patient {FullName} already has ongoing hospitalization.");
            var hospitalization = new Hospitalization
            {
                Patient = this,
                Start = start,
                Diagnosis = diagnosis
            };
            Hospitalizations.AddLast(hospitalization);
            return hospitalization;
        }

        public Hospitalization EndHospitalization(DateTime end)
        {
            var lastHospitalization = Hospitalizations.Last.Value;
            if (lastHospitalization.IsDone) throw new Exception($"Patient {FullName} hasn't ongoing hospitalization.");
            lastHospitalization.End = end;
            return lastHospitalization;
        }

        public Hospitalization LoadHospitalization(Hospitalization hospitalization)
        {
            hospitalization.Patient = this;
            Hospitalizations.AddLast(hospitalization);
            return hospitalization;
        }

        public string[] ToArray()
        {
            return new[]
            {
                Name,
                Surname,
                Birthday.ToString(),
                PersonalNumber.ToString(),
                AgencyCode
            };
        }

        public override string ToString() => $"{FullName}, {PersonalNumber}, {AgencyCode}";
    }
}
