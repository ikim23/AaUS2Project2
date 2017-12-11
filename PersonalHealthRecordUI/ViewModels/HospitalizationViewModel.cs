using System;
using Caliburn.Micro;
using PersonalHealthRecordUI.Models;

namespace PersonalHealthRecordUI.ViewModels
{
    public class HospitalizationViewModel : Screen
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Diagnosis { get; set; }
        public bool NewHospitalization { get; }
        public bool OldHospitalization => !NewHospitalization;
        public bool CanEnd => OldHospitalization && EndDate > StartDate;
        private readonly int _cardId;
        private readonly IOnHospitalizationViewListener _listener;

        public HospitalizationViewModel(int cardId, IOnHospitalizationViewListener listener)
        {
            _cardId = cardId;
            _listener = listener;
            NewHospitalization = true;
            StartDate = DateTime.Today;
        }

        public HospitalizationViewModel(int cardId, IOnHospitalizationViewListener listener, HospitalizationModel hospitalization)
        {
            _cardId = cardId;
            _listener = listener;
            NewHospitalization = false;
            StartDate = hospitalization.StartDate;
            Diagnosis = hospitalization.Diagnosis;
        }

        public bool CanCreate(DateTime startDate, string diagnosis)
        {
            return NewHospitalization && !string.IsNullOrWhiteSpace(diagnosis);
        }

        public void Create(DateTime startDate, string diagnosis)
        {
            _listener.OnHospitalizationCreate(_cardId, startDate, diagnosis);
            TryClose();
        }

        public void End(DateTime endDate)
        {
            _listener.OnHospitalizationEnd(_cardId, endDate);
            TryClose();
        }
    }

    public interface IOnHospitalizationViewListener
    {
        void OnHospitalizationCreate(int cardId, DateTime startDate, string diagnosis);
        void OnHospitalizationEnd(int cardId, DateTime endDate);
    }
}
