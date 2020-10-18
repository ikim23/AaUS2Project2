using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using PersonalHealthRecordUI.Logic;
using PersonalHealthRecordUI.Models;

namespace PersonalHealthRecordUI.ViewModels
{
    public sealed class SearchHospitalizationViewModel : Screen, IShellTabItem
    {
        public BindableCollection<HospitalModel> Hospitals { get; set; }
        public BindableCollection<HospitalizationModel> Hospitalizations { get; set; }
        private readonly IApi _api = Api.Instance;

        public SearchHospitalizationViewModel()
        {
            DisplayName = "Search Hospitalizations";
            Hospitals = new BindableCollection<HospitalModel>();
            Hospitalizations = new BindableCollection<HospitalizationModel>();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Hospitals.Clear();
            Hospitals.AddRange(_api.GetHospitals());
            Hospitalizations.Clear();
        }

        public bool CanSearch(HospitalModel hospital, bool searchByPersonalNumber, bool searchByFullName, int personalNumber, string fullName)
        {
            if (string.IsNullOrEmpty(hospital?.Name)) return false;
            if (searchByPersonalNumber) return personalNumber > 0;
            if (searchByFullName) return !string.IsNullOrEmpty(fullName);
            return false;
        }

        public void Search(HospitalModel hospital, bool searchByPersonalNumber, bool searchByFullName, int personalNumber, string fullName)
        {
            try
            {
                if (searchByPersonalNumber)
                {
                    var hospitalizations = _api.GetHospitalizations(hospital.Name, personalNumber);
                    RefreshHospitalizations(hospitalizations);
                }
                else if (searchByFullName)
                {
                    var name = fullName.Split();
                    var hospitalizations = _api.GetHospitalizations(hospital.Name, name[0], name[1]);
                    RefreshHospitalizations(hospitalizations);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void RefreshHospitalizations(IEnumerable<HospitalizationModel> hospitalizations)
        {
            Hospitalizations.Clear();
            Hospitalizations.AddRange(hospitalizations);
        }
    }
}
