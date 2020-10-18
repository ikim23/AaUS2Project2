using System.Windows;
using Caliburn.Micro;
using PersonalHealthRecordUI.ViewModels;

namespace PersonalHealthRecordUI
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }
        
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
