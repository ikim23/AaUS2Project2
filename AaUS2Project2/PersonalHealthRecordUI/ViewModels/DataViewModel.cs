using System;
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

        public bool CanGenerate(int blockSize, int patientsCount, int recordsCount, int ongoingRecordsCount)
        {
            return blockSize >= 3 &&
                patientsCount > 0 &&
                recordsCount > 0 &&
                ongoingRecordsCount <= patientsCount;
        }

        public void Generate(int blockSize, int patientsCount, int recordsCount, int ongoingRecordsCount)
        {
            try
            {
                _api.Generate(blockSize, patientsCount, recordsCount, ongoingRecordsCount);
                MessageBox.Show("Done!");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
