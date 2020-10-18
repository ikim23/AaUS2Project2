using System;
using Caliburn.Micro;
using ChatClient.Models;

namespace ChatClient.ViewModels
{
    public class SettingsViewModel : Screen
    {
        public SettingsModel Settings { get; set; }
        private readonly Action<SettingsModel> _onSettingsChange;

        public SettingsViewModel(SettingsModel settings, Action<SettingsModel> onSettingsChange)
        {
            Settings = settings;
            _onSettingsChange = onSettingsChange;
        }

        public void Ok()
        {
            _onSettingsChange(Settings);
            TryClose();
        }

        public void Cancel()
        {
            TryClose();
        }
    }
}
