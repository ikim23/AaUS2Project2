using System;
using System.Dynamic;
using System.Windows;
using Caliburn.Micro;
using PersonalHealthRecordUI.Logic;
using PersonalHealthRecordUI.Models;

namespace PersonalHealthRecordUI.ViewModels
{
    public sealed class SearchPatientViewModel : Screen, IShellTabItem
    {
        public HospitalModel Hospital
        {
            get => _hospital;
            set
            {
                _hospital = value;
                NotifyOfPropertyChange(() => Hospital);
                Patients.Clear();
            }
        }
        public bool SearchByHospitalName
        {
            get => _searchByHospitalName;
            set
            {
                _searchByHospitalName = value;
                NotifyOfPropertyChange(() => SearchByHospitalName);
                Patients.Clear();
            }
        }
        public bool SearchByRange
        {
            get => _searchByRange;
            set
            {
                _searchByRange = value;
                NotifyOfPropertyChange(() => SearchByRange);
                Patients.Clear();
            }
        }
        public bool SearchByInsuranceAgencyCode
        {
            get => _searchByInsuranceAgencyCode;
            set
            {
                _searchByInsuranceAgencyCode = value;
                NotifyOfPropertyChange(() => SearchByInsuranceAgencyCode);
                Patients.Clear();
            }
        }
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                NotifyOfPropertyChange(() => StartDate);
                Patients.Clear();
            }
        }
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                NotifyOfPropertyChange(() => EndDate);
                Patients.Clear();
            }
        }
        public string InsuranceAgencyCode
        {
            get => _insuranceAgencyCode;
            set
            {
                _insuranceAgencyCode = value;
                NotifyOfPropertyChange(() => InsuranceAgencyCode);
                Patients.Clear();
            }
        }
        public BindableCollection<HospitalModel> Hospitals { get; set; }
        public BindableCollection<PatientModel> Patients { get; set; }
        public PatientModel SelectedPatient { get; set; }
        private HospitalModel _hospital;
        private bool _searchByHospitalName;
        private bool _searchByRange;
        private bool _searchByInsuranceAgencyCode;
        private DateTime _startDate;
        private DateTime _endDate;
        private string _insuranceAgencyCode;
        private readonly IApi _api = Api.Instance;

        public SearchPatientViewModel()
        {
            DisplayName = "Search Patients";
            _searchByHospitalName = true;
            _startDate = _api.SysDate.AddMonths(-1);
            _endDate = _api.SysDate;
            Hospitals = new BindableCollection<HospitalModel>();
            Patients = new BindableCollection<PatientModel>();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Hospitals.Clear();
            Hospitals.AddRange(_api.GetHospitals());
            Patients.Clear();
        }

        public bool CanSearch(HospitalModel hospital, bool searchByHospitalName, bool searchByRange, bool searchByInsuranceAgencyCode, DateTime startDate, DateTime endDate, string insuranceAgencyCode)
        {
            if (string.IsNullOrEmpty(hospital?.Name)) return false;
            if (searchByHospitalName) return true;
            if (searchByRange) return DateTime.Compare(startDate, endDate) < 0;
            if (searchByInsuranceAgencyCode) return !string.IsNullOrEmpty(insuranceAgencyCode);
            return false;
        }

        public void Search(HospitalModel hospital, bool searchByHospitalName, bool searchByRange, bool searchByInsuranceAgencyCode, DateTime startDate, DateTime endDate, string insuranceAgencyCode)
        {
            try
            {
                Patients.Clear();
                if (searchByHospitalName)
                {
                    var patients = _api.GetHospitalizedPatients(hospital.Name);
                    Patients.AddRange(patients);
                }
                else if (searchByRange)
                {
                    var start = new DateTime(startDate.Year, startDate.Month, startDate.Day);
                    var end = endDate.AddDays(1).AddMilliseconds(-1);
                    var patients = _api.GetPatients(hospital.Name, start, end);
                    Patients.AddRange(patients);
                }
                else if (searchByInsuranceAgencyCode)
                {
                    var patients = _api.GetHospitalizedPatients(hospital.Name, insuranceAgencyCode);
                    Patients.AddRange(patients);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
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
