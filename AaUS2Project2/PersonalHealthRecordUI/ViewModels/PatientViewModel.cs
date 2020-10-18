using System;
using Caliburn.Micro;
using PersonalHealthRecord.Generator;
using PersonalHealthRecordUI.Models;

namespace PersonalHealthRecordUI.ViewModels
{
    public class PatientViewModel : Screen
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public int CardId { get; set; }
        public bool NewPatient { get; }
        public bool CanRemove => !NewPatient;
        private readonly IOnPatientViewListener _listener;
        private readonly int _oldCardId;

        public PatientViewModel(IOnPatientViewListener listener)
        {
            NewPatient = true;
            _listener = listener;
            var rand = RandomValue.Instance;
            FirstName = rand.GetFirstName();
            LastName = rand.GetLastName();
            Birthday = rand.GetBirthday();
        }

        public PatientViewModel(IOnPatientViewListener listener, PatientModel patient)
        {
            NewPatient = false;
            _listener = listener;
            FirstName = patient.FirstName;
            LastName = patient.LastName;
            Birthday = patient.Birthday;
            _oldCardId = CardId = patient.CardId;
        }

        public bool CanOk(string firstName, string lastName, DateTime birthday, int cardId)
        {
            return !string.IsNullOrWhiteSpace(firstName) &&
                   !string.IsNullOrWhiteSpace(lastName) &&
                   cardId > 0 &&
                   birthday < DateTime.Now;
        }

        public void Ok(string firstName, string lastName, DateTime birthday, int cardId)
        {
            var patient = new PatientModel
            {
                FirstName = firstName,
                LastName = lastName,
                Birthday = birthday,
                CardId = cardId
            };
            if (NewPatient) _listener.OnPatientAdd(patient);
            else if (_oldCardId == cardId) _listener.OnPatientEdit(patient);
            else _listener.OnPatientEdit(_oldCardId, patient);
            TryClose();
        }

        public void Cancel()
        {
            TryClose();
        }

        public void Remove()
        {
            _listener.OnPatientRemove(CardId);
            TryClose();
        }
    }

    public interface IOnPatientViewListener
    {
        void OnPatientAdd(PatientModel patient);
        void OnPatientEdit(PatientModel patient);
        void OnPatientEdit(int oldCardId, PatientModel patient);
        void OnPatientRemove(int cardId);
    }
}
