using System.Windows;
using Caliburn.Micro;
using PersonalHealthRecordUI.Logic;

namespace PersonalHealthRecordUI.ViewModels
{
    public sealed class DataViewModel : Screen, IShellTabItem
    {
        private readonly Api _api = Api.Instance;

        public DataViewModel()
        {
            DisplayName = "Data";
        }

        public bool CanGenerate(int patientsCount, int recordsCount, int ongoingRecordsCount)
        {
            return patientsCount > 0 &&
                recordsCount > 0 &&
                ongoingRecordsCount <= patientsCount;
        }

        public void Generate(int patientsCount, int recordsCount, int ongoingRecordsCount)
        {
            _api.Generate(patientsCount, recordsCount, ongoingRecordsCount);
            MessageBox.Show("Done!");
        }
    }
}
