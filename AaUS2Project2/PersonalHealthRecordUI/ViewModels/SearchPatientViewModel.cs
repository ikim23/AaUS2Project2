using System;
using System.Windows;
using Caliburn.Micro;
using PersonalHealthRecordUI.Logic;
using PersonalHealthRecordUI.Models;

namespace PersonalHealthRecordUI.ViewModels
{
    public sealed class SearchPatientViewModel : Screen, IShellTabItem
    {
        public BindableCollection<PatientModel> Patients { get; set; }
        private readonly Api _api = Api.Instance;

        public SearchPatientViewModel()
        {
            DisplayName = "Search Patients";
            Patients = new BindableCollection<PatientModel>();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Patients.Clear();
        }

        public bool CanSearch(int cardIdFrom, int cardIdTo) => cardIdFrom > 0 && cardIdFrom < cardIdTo;

        public void Search(int cardIdFrom, int cardIdTo)
        {
            try
            {
                Patients.Clear();
                var patients = _api.GetPatients(cardIdFrom, cardIdTo);
                Patients.AddRange(patients);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
