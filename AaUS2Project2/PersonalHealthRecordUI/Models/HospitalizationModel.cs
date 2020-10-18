using System;

namespace PersonalHealthRecordUI.Models
{
    public class HospitalizationModel
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Diagnosis { get; set; }
    }
}
