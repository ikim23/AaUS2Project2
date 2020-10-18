using System;
using Caliburn.Micro;
using Action = System.Action;

namespace ChatClient.ViewModels
{
    public class LoginViewModel : Screen
    {
        private readonly Func<string, bool> _onOk;
        private readonly Action _onCancel;

        public LoginViewModel(Func<string, bool> onOk, Action onCancel)
        {
            _onOk = onOk;
            _onCancel = onCancel;
        }

        public bool CanOk(string name) => !string.IsNullOrWhiteSpace(name);

        public void Ok(string name)
        {
            if (_onOk(name))
            {
                TryClose();
            }
        }

        public void Cancel()
        {
            _onCancel();
            TryClose();
        }
    }
}
