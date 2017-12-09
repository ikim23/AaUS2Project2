using System;
using System.Windows;
using Caliburn.Micro;
using PersonalHealthRecordUI.Logic;
using PersonalHealthRecordUI.Models;

namespace PersonalHealthRecordUI.ViewModels
{
    public sealed class HospitalizationsViewModel : Screen, IShellTabItem, IOnHospitalizationViewListener
    {
        public BindableCollection<HospitalizationModel> Hospitalizations { get; set; }
        public HospitalizationModel SelectedHospitalization { get; set; }
        public int CardId { get; set; }
        private readonly Api _api = Api.Instance;

        public HospitalizationsViewModel()
        {
            DisplayName = "Hospitalizations";
            Hospitalizations = new BindableCollection<HospitalizationModel>();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Hospitalizations.Clear();
        }

        public bool CanSearch(int cardId) => cardId > 0;

        public void Search(int cardId)
        {
            try
            {
                RefreshHospitalizations(cardId);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public bool CanAdd(int cardId) => cardId > 0;

        public void Add(int cardId)
        {
            var windowManager = new WindowManager();
            windowManager.ShowWindow(new HospitalizationViewModel(cardId, this));
        }

        public void OnHospitalizationCreate(int cardId, DateTime startDate, string diagnosis)
        {
            try
            {
                _api.StartHospitalization(cardId, startDate, diagnosis);
                RefreshHospitalizations(cardId);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void OnHospitalizationDoubleClick()
        {
            if (SelectedHospitalization?.EndDate != null) return;
            var windowManager = new WindowManager();
            windowManager.ShowWindow(new HospitalizationViewModel(CardId, this, SelectedHospitalization));
        }

        public void OnHospitalizationEnd(int cardId, DateTime endDate)
        {
            try
            {
                _api.EndHospitalization(cardId, endDate);
                RefreshHospitalizations(cardId);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void RefreshHospitalizations(int cardId)
        {
            Hospitalizations.Clear();
            var hospitalizations = _api.GetHospitalizations(cardId);
            Hospitalizations.AddRange(hospitalizations);
        }
    }
}
