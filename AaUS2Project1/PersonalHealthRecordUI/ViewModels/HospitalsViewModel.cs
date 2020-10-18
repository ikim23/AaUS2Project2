using System;
using System.Windows;
using Caliburn.Micro;
using PersonalHealthRecordUI.Logic;
using PersonalHealthRecordUI.Models;

namespace PersonalHealthRecordUI.ViewModels
{
    public sealed class HospitalsViewModel : Screen, IShellTabItem
    {
        public string NewHospitalName
        {
            get => _newHospitalName;
            set
            {
                _newHospitalName = value;
                NotifyOfPropertyChange(NewHospitalName);
            }
        }
        public BindableCollection<HospitalModel> Hospitals { get; set; }
        private string _newHospitalName;
        private readonly IApi _api = Api.Instance;

        public HospitalsViewModel()
        {
            DisplayName = "Hospitals";
            Hospitals = new BindableCollection<HospitalModel>();
        }

        protected override void OnActivate()
        {
            RefreshHospitals();
        }

        public bool CanAddHospital(string newHospitalName) => !string.IsNullOrEmpty(newHospitalName);

        public void AddHospital(string newHospitalName)
        {
            try
            {
                _api.AddHospital(newHospitalName);
                NewHospitalName = string.Empty;
                RefreshHospitals();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public bool CanRemoveHospital(HospitalModel oldHospital, HospitalModel newHospital)
        {
            return !string.IsNullOrEmpty(oldHospital?.Name) &&
                   !string.IsNullOrEmpty(newHospital?.Name) &&
                   oldHospital.Name != newHospital.Name;
        }

        public void RemoveHospital(HospitalModel oldHospital, HospitalModel newHospital)
        {
            _api.RemoveHospital(oldHospital.Name, newHospital.Name);
            RefreshHospitals();
        }

        private void RefreshHospitals()
        {
            Hospitals.Clear();
            Hospitals.AddRange(_api.GetHospitals());
        }
    }
}
