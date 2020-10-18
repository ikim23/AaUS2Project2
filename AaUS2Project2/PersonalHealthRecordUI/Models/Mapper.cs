using PersonalHealthRecord.Model;

namespace PersonalHealthRecordUI.Models
{
    internal class Mapper
    {
        public static PatientModel ToPatientModel(string[] args)
        {
            return new PatientModel
            {
                FirstName = args[0],
                LastName = args[1],
                Birthday = DateFormatter.Parse(args[2]),
                CardId = int.Parse(args[3])
            };
        }

        public static HospitalizationModel ToHospitalizationModel(string[] args)
        {
            return new HospitalizationModel
            {
                StartDate = DateFormatter.Parse(args[0]),
                EndDate = DateFormatter.ParseOrNull(args[1]),
                Diagnosis = args[2]
            };
        }
    }
}
