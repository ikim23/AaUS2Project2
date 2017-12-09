﻿using System.Dynamic;
using Caliburn.Micro;
using PersonalHealthRecordUI.Logic;
using PersonalHealthRecordUI.Models;

namespace PersonalHealthRecordUI.ViewModels
{
    public sealed class PatientsViewModel : Screen, IShellTabItem, IOnPatientViewListener
    {
        public BindableCollection<PatientModel> Patients { get; set; }
        public PatientModel SelectedPatient { get; set; }
        private readonly Api _api = Api.Instance;

        public PatientsViewModel()
        {
            DisplayName = "Patients";
            Patients = new BindableCollection<PatientModel>();
        }

        protected override void OnActivate()
        {
            RefreshPatients();
        }

        public void AddPatient()
        {
            var windowManager = new WindowManager();
            dynamic settings = new ExpandoObject();
            settings.Title = "New Patient";
            windowManager.ShowWindow(new PatientViewModel(this), settings: settings);
        }

        public void OnPatientAdd(PatientModel patient)
        {
            _api.AddPatient(patient);
            RefreshPatients();
        }

        public void OnPatientDoubleClick()
        {
            var windowManager = new WindowManager();
            dynamic settings = new ExpandoObject();
            settings.Title = "Edit Patient";
            windowManager.ShowWindow(new PatientViewModel(this, SelectedPatient), settings: settings);
        }

        public void OnPatientEdit(PatientModel patient)
        {
            _api.UpdatePatient(patient);
            RefreshPatients();
        }

        private void RefreshPatients()
        {
            Patients.Clear();
            Patients.AddRange(_api.GetPatients());
        }
    }
}
