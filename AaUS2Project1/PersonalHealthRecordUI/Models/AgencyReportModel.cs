using Caliburn.Micro;

namespace PersonalHealthRecordUI.Models
{
    public class AgencyReportModel : Screen
    {
        public string AgencyCode { get; set; }
        public int Count => Hospitalizations.Count;
        public BindableCollection<AgencyReportItemModel> Hospitalizations { get; internal set; }

        public AgencyReportModel()
        {
            Hospitalizations = new BindableCollection<AgencyReportItemModel>();
        }
    }
}
