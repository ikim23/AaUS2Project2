using System;

namespace PersonalHealthRecordUI.Models
{
    public class PatientModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public DateTime Birthday { get; set; }
        public int PersonalNumber { get; set; }
        public string InsuranceAgencyCode { get; set; }

        public PatientModel()
        {
        }

        public PatientModel(string name, string surname, DateTime birthday, int personalNumber, string agencyCode)
        {
            FirstName = name;
            LastName = surname;
            Birthday = birthday;
            PersonalNumber = personalNumber;
            InsuranceAgencyCode = agencyCode;
        }

        public PatientModel(string name, string surname, string birthday, string personalNumber, string agencyCode) :
            this(name, surname, DateTime.Parse(birthday), int.Parse(personalNumber), agencyCode)
        {
        }
    }
}
