using System;

namespace PersonalHealthRecordUI.Models
{
    public class HospitalizationModel
    {
        public string Hospital { get; set; }
        public PatientModel Patient { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Diagnosis { get; set; }

        public HospitalizationModel(string hospital, string start, string end, string diagnosis)
        {
            Hospital = hospital;
            StartDate = DateTime.Parse(start);
            if (end != null) EndDate = DateTime.Parse(end);
            Diagnosis = diagnosis;
        }

        public HospitalizationModel(string firstName, string lastName, string personalNumber, string hospital, string start, string end, string diagnosis)
        {
            Patient = new PatientModel
            {
                FirstName = firstName,
                LastName = lastName,
                PersonalNumber = int.Parse(personalNumber)
            };
            Hospital = hospital;
            StartDate = DateTime.Parse(start);
            if (end != null) EndDate = DateTime.Parse(end);
            Diagnosis = diagnosis;
        }
    }
}
