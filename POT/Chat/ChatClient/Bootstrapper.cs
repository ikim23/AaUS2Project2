using System;
using System.Dynamic;
using System.Windows;
using Caliburn.Micro;
using ChatClient.Messages;
using ChatClient.ViewModels;

namespace ChatClient
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly EventAggregator _aggregator;
        private readonly DataFetcher _dataFetcher;

        public Bootstrapper()
        {
            _aggregator = new EventAggregator();
            _dataFetcher = new DataFetcher(_aggregator);
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            var windowManager = new WindowManager();
            var window = new LoginViewModel(LogIn, CancelLogIn);
            windowManager.ShowWindow(window);
            //AutoLogIn();
        }

        private void AutoLogIn()
        {
            var userId = $"user_{DateTime.Now.Ticks}";
            if (_dataFetcher.LogIn(userId))
            {
                var windowManager = new WindowManager();
                var mainWindow = new MainViewModel(_aggregator);
                dynamic setting = new ExpandoObject();
                setting.Title = $"Chat: {userId}";
                windowManager.ShowWindow(mainWindow, settings: setting);
            }
        }

        private bool LogIn(string name)
        {
            if (!_dataFetcher.LogIn(name)) return false;
            var windowManager = new WindowManager();
            var mainWindow = new MainViewModel(_aggregator);
            dynamic setting = new ExpandoObject();
            setting.Title = $"Chat: {name}";
            windowManager.ShowWindow(mainWindow, settings: setting);
            return true;
        }

        private void CancelLogIn()
        {
            _dataFetcher.Handle(new DisconnectMessage());
        }
    }
}
