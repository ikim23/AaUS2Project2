using System.Windows;
using Caliburn.Micro;
using HockeyPlayerDatabase.MainApp.ViewModels;

namespace HockeyPlayerDatabase.MainApp
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }
    }
}
