using System;
using System.Windows;
using Caliburn.Micro;
using PersonalHealthRecordUI.Logic;

namespace PersonalHealthRecordUI.ViewModels
{
    public sealed class DataViewModel : Screen, IShellTabItem
    {
        public DateTime SysDate
        {
            get => _sysDate;
            set
            {
                if (_sysDate < value) _api.SysDate = _sysDate = value;
                else MessageBox.Show("System time can be changes only forwards.");
            }
        }

        private DateTime _sysDate = DateTime.Today;
        private readonly IApi _api = Api.Instance;

        public DataViewModel()
        {
            DisplayName = "Data";
        }

        public bool CanGenerate(int hospitalsCount, int patientsCount, int recordsCount, int ongoingRecordsCount)
        {
            return hospitalsCount > 0 &&
                patientsCount > 0 &&
                recordsCount > 0 &&
                ongoingRecordsCount <= patientsCount;
        }

        public void Generate(int hospitalsCount, int patientsCount, int recordsCount, int ongoingRecordsCount)
        {
            _api.Generate(hospitalsCount, patientsCount, recordsCount, ongoingRecordsCount);
            MessageBox.Show("Done!");
        }
    }
}
