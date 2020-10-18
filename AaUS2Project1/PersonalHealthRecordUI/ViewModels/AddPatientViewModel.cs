using System;
using System.Windows;
using Caliburn.Micro;
using PersonalHealthRecord.Generator;
using PersonalHealthRecordUI.Logic;
using PersonalHealthRecordUI.Models;

namespace PersonalHealthRecordUI.ViewModels
{
    public delegate void OnAddPatient(PatientModel patient);

    public class AddPatientViewModel : Screen
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public int PersonalNumber { get; set; }
        public string InsuranceAgencyCode { get; set; }
        private readonly OnAddPatient _onAddPatient;
        private readonly IApi _api = Api.Instance;

        public AddPatientViewModel(OnAddPatient onAddPatient)
        {
            _onAddPatient = onAddPatient;
            var rand = RandomValue.Instance;
            FirstName = rand.GetFirstName();
            LastName = rand.GetLastName();
            Birthday = rand.GetBirthday();
            //PersonalNumber = ?;
            InsuranceAgencyCode = rand.GeInsuranceAgencyCode();
        }

        public bool CanOk(string firstName, string lastName, DateTime birthday, int personalNumber, string insuranceAgencyCode)
        {
            return !string.IsNullOrEmpty(firstName) &&
                   !string.IsNullOrEmpty(lastName) &&
                   personalNumber > 0 &&
                   !string.IsNullOrEmpty(insuranceAgencyCode) &&
                   Birthday < DateTime.Now;
        }

        public void Ok(string firstName, string lastName, DateTime birthday, int personalNumber, string insuranceAgencyCode)
        {
            try
            {
                var patient = new PatientModel(firstName, lastName, birthday, personalNumber, insuranceAgencyCode);
                _api.AddPatient(patient);
                _onAddPatient(patient);
                TryClose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void Cancel()
        {
            TryClose();
        }
    }
}
