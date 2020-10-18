using System;
using Caliburn.Micro;
using PersonalHealthRecordUI.Logic;
using PersonalHealthRecordUI.Models;

namespace PersonalHealthRecordUI.ViewModels
{
    public sealed class AgencyReportViewModel : Screen, IShellTabItem
    {
        public DateTime Month { get; set; }
        public BindableCollection<HospitalModel> Hospitals { get; set; }
        public BindableCollection<AgencyReportModel> Reports { get; set; }
        private readonly IApi _api = Api.Instance;

        public AgencyReportViewModel()
        {
            DisplayName = "Agency Report";
            Month = _api.SysDate;
            Hospitals = new BindableCollection<HospitalModel>();
            Reports = new BindableCollection<AgencyReportModel>();
        }

        protected override void OnActivate()
        {
            Hospitals.Clear();
            Hospitals.AddRange(_api.GetHospitals());
            Reports.Clear();
        }

        public bool CanShow(DateTime month, HospitalModel hospital)
        {
            return month <= _api.SysDate &&
                   !string.IsNullOrEmpty(hospital?.Name);
        }

        public void Show(DateTime month, HospitalModel hospital)
        {
            var reports = _api.GetAccountantReport(hospital.Name, month);
            Reports.Clear();
            Reports.AddRange(reports);
        }
    }
}
