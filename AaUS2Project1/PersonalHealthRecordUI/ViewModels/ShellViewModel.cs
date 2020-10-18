using System.Collections.Generic;
using Caliburn.Micro;
using PersonalHealthRecordUI.Logic;

namespace PersonalHealthRecordUI.ViewModels
{
    public class ShellViewModel : Conductor<IShellTabItem>.Collection.OneActive
    {
        private readonly IApi _api = Api.Instance;

        public ShellViewModel()
        {
            _api.Load();
            IEnumerable<IShellTabItem> tabItems = new List<IShellTabItem>
            {
                new HospitalsViewModel(),
                new PatientsViewModel(),
                new SearchPatientViewModel(),
                new SearchHospitalizationViewModel(),
                new AgencyReportViewModel(),
                new DataViewModel()
            };
            Items.AddRange(tabItems);
        }

        protected override void OnDeactivate(bool close)
        {
            _api.Save();
        }
    }
}
