using System.Collections.Generic;
using Caliburn.Micro;
using PersonalHealthRecordUI.Logic;

namespace PersonalHealthRecordUI.ViewModels
{
    public class ShellViewModel : Conductor<IShellTabItem>.Collection.OneActive
    {
        public ShellViewModel()
        {
            // init API instance
            var api = Api.Instance;
            IEnumerable<IShellTabItem> tabItems = new List<IShellTabItem>
            {
                new PatientsViewModel(),
                new HospitalizationsViewModel(),
                new SearchPatientViewModel(),
                new DataViewModel()
            };
            Items.AddRange(tabItems);
        }
    }
}
