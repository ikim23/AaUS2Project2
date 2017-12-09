using System;

namespace PersonalHealthRecord.Model
{
    internal class Mapper
    {
        public static string FormatDate(DateTime? date) => date != null ? $"{date:dd.MM.yyyy}" : "";

        public static string[] FromPatient(Patient patient)
        {
            return new[]
            {
                patient.FirstName,
                patient.LastName,
                FormatDate(patient.Birthday),
                patient.CardId.ToString()
            };
        }

        public static Patient ToPatient(string firstName, string lastName, DateTime birthday, int cardId)
        {
            return new Patient
            {
                FirstName = firstName,
                LastName = lastName,
                Birthday = birthday,
                CardId = cardId
            };
        }

        public static string[] FromHospitalization(Hospitalization hospitalization)
        {
            return new[]
            {
                FormatDate(hospitalization.Start),
                FormatDate(hospitalization.End),
                hospitalization.Diagnosis
            };
        }

        public static Hospitalization ToHospitalization(DateTime start, DateTime? end, string diagnosis)
        {
            return new Hospitalization
            {
                Start = start,
                End = end,
                Diagnosis = diagnosis
            };
        }
    }
}
