using System;
using System.Windows;
using Caliburn.Micro;
using PersonalHealthRecordUI.Logic;
using PersonalHealthRecordUI.Models;

namespace PersonalHealthRecordUI.ViewModels
{
    public class HospitalizationsViewModel : Screen
    {
        public string HospitalName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Diagnosis { get; set; }

        public HospitalModel Hospital { get; set; }
        public HospitalizationModel Hospitalization { get; set; }
        public BindableCollection<HospitalModel> Hospitals { get; set; }
        public BindableCollection<HospitalizationModel> Hospitalizations { get; set; }

        public Visibility HospitalComboBoxVisibility { get; set; }
        public Visibility HospitalNameVisibility { get; set; }
        public bool CanEditDiagnosis { get; set; }
        public bool CanEnd { get; set; }

        private readonly int _personalNumber;
        private readonly IApi _api = Api.Instance;

        public HospitalizationsViewModel(int personalNumber)
        {
            Hospitals = new BindableCollection<HospitalModel>();
            Hospitalizations = new BindableCollection<HospitalizationModel>();
            _personalNumber = personalNumber;
            New();
            Hospitals.AddRange(_api.GetHospitals());
            RefreshHospitalizations();
        }

        public void New()
        {
            HospitalComboBoxVisibility = Visibility.Visible;
            HospitalNameVisibility = Visibility.Hidden;
            CanEditDiagnosis = true;
            CanEnd = false;

            Hospital = null;
            StartDate = $"{_api.SysDate:dd. MM. yyyy}";
            EndDate = null;
            Diagnosis = "Lorem ipsum dolor sit amet.";

            NotifyAll();
        }

        public bool CanCreate(HospitalModel hospital, string diagnosis)
        {
            return !string.IsNullOrEmpty(hospital?.Name) &&
                   !string.IsNullOrEmpty(diagnosis);
        }

        public void Create(HospitalModel hospital, string diagnosis)
        {
            try
            {
                _api.StartHospitalization(hospital.Name, _personalNumber, diagnosis);
                New();
                RefreshHospitalizations();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void End()
        {
            _api.EndHospitalization(HospitalName, _personalNumber);
            New();
            RefreshHospitalizations();
        }

        public void OnHospitalizationClick()
        {
            HospitalComboBoxVisibility = Visibility.Hidden;
            HospitalNameVisibility = Visibility.Visible;
            CanEditDiagnosis = false;
            CanEnd = Hospitalization.EndDate == null;

            HospitalName = Hospitalization.Hospital;
            StartDate = $"{Hospitalization.StartDate:dd. MM. yyyy}";
            EndDate = Hospitalization.EndDate == null ? null : $"{Hospitalization.EndDate:dd. MM. yyyy}";
            Diagnosis = Hospitalization.Diagnosis;

            NotifyAll();
        }

        private void RefreshHospitalizations()
        {
            Hospitalizations.Clear();
            var hospitalizations = _api.GetHospitalizations(_personalNumber);
            Hospitalizations.AddRange(hospitalizations);
        }

        private void NotifyAll()
        {
            NotifyOfPropertyChange(() => Hospital);

            NotifyOfPropertyChange(() => HospitalName);
            NotifyOfPropertyChange(() => StartDate);
            NotifyOfPropertyChange(() => EndDate);
            NotifyOfPropertyChange(() => Diagnosis);

            NotifyOfPropertyChange(() => HospitalComboBoxVisibility);
            NotifyOfPropertyChange(() => HospitalNameVisibility);
            NotifyOfPropertyChange(() => CanEditDiagnosis);
            NotifyOfPropertyChange(() => CanEnd);
        }
    }
}
