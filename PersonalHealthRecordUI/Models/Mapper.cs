using System;
using System.Globalization;

namespace PersonalHealthRecordUI.Models
{
    internal class Mapper
    {
        public static DateTime Parse(string date) => DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);

        public static DateTime? ParseOrNull(string date)
        {
            if (string.IsNullOrEmpty(date)) return null;
            return DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        }

        public static PatientModel ToPatientModel(string[] args)
        {
            return new PatientModel
            {
                FirstName = args[0],
                LastName = args[1],
                Birthday = Parse(args[2]),
                CardId = int.Parse(args[3])
            };
        }

        public static HospitalizationModel ToHospitalizationModel(string[] args)
        {
            return new HospitalizationModel
            {
                StartDate = Parse(args[0]),
                EndDate = ParseOrNull(args[1]),
                Diagnosis = args[2]
            };
        }
    }
}
