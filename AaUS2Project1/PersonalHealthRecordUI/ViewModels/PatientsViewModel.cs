using System.Dynamic;
using Caliburn.Micro;
using PersonalHealthRecordUI.Logic;
using PersonalHealthRecordUI.Models;

namespace PersonalHealthRecordUI.ViewModels
{
    public sealed class PatientsViewModel : Screen, IShellTabItem
    {
        public BindableCollection<PatientModel> Patients { get; set; }
        public PatientModel SelectedPatient { get; set; }
        private readonly IApi _api = Api.Instance;

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
            windowManager.ShowWindow(new AddPatientViewModel(OnAddPatient));
        }

        private void OnAddPatient(PatientModel patient)
        {
            RefreshPatients();
        }

        private void RefreshPatients()
        {
            Patients.Clear();
            Patients.AddRange(_api.GetPatients());
        }

        public void OnPatientDoubleClick()
        {
            var windowManager = new WindowManager();
            dynamic settings = new ExpandoObject();
            settings.Title = $"{SelectedPatient.FirstName} {SelectedPatient.LastName}";
            windowManager.ShowWindow(rootModel: new HospitalizationsViewModel(SelectedPatient.PersonalNumber), settings: settings);
        }
    }
}
