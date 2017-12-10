using System;

namespace PersonalHealthRecordUI.Models
{
    public class PatientModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public int CardId { get; set; }
    }
}
