using System;
using System.Collections.Generic;
using System.IO;
using Caliburn.Micro;
using ChatClient.Messages;
using ChatClient.Models;
using ChatLibrary;
using Microsoft.Win32;

namespace ChatClient.ViewModels
{
    public class MainViewModel : Screen, IHandle<OnlineUsersMessage>, IHandle<NewMessagesMessage>
    {
        public BindableCollection<string> OnlineUsers { get; set; } = new BindableCollection<string>();
        public BindableCollection<string> Recipients { get; set; } = new BindableCollection<string>();
        public BindableCollection<IMessageModel> Messages { get; set; } = new BindableCollection<IMessageModel>();
        public SettingsModel Settings { get; set; } = new SettingsModel();
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }
        public string SelectedRecipient
        {
            get => _selectedRecipient;
            set
            {
                _selectedRecipient = value;
                NotifyOfPropertyChange(() => SelectedRecipient);
            }
        }
        private readonly IEventAggregator _aggregator;
        private string _selectedRecipient;
        private string _message;

        public MainViewModel(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            aggregator.Subscribe(this);
            SetRecipients(new List<string>());
        }

        public bool CanSend(string message) => !string.IsNullOrWhiteSpace(message);

        public void Send(string message)
        {
            var msg = new TextMessage { Message = message };
            if (SelectedRecipient != "All") msg.Receiver = SelectedRecipient;
            _aggregator.PublishOnBackgroundThread(new SendMessage { Message = msg });
            Message = null;
        }

        public void SendImage()
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg)|*.png;*.jpg",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            var result = openDialog.ShowDialog();
            if (result == null || !result.Value) return;
            var fileName = openDialog.FileName;
            var bytes = File.ReadAllBytes(fileName);
            var msg = new ImageMessage { Image = bytes };
            if (SelectedRecipient != "All") msg.Receiver = SelectedRecipient;
            _aggregator.PublishOnBackgroundThread(new SendMessage { Message = msg });
        }

        public void OpenSettings()
        {
            var windowManager = new WindowManager();
            var window = new SettingsViewModel(Settings, OnSettingsChange);
            windowManager.ShowWindow(window);
        }

        private void OnSettingsChange(SettingsModel settings)
        {
            Settings = settings;
            foreach (var message in Messages)
                message.UpdateStyle(settings);
            Messages.Refresh();
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
                _aggregator.PublishOnBackgroundThread(new DisconnectMessage());
        }

        public void Handle(OnlineUsersMessage msg)
        {
            OnlineUsers.Clear();
            OnlineUsers.AddRange(msg.OnlineUsers);
            SetRecipients(msg.OnlineUsers);
        }

        private void SetRecipients(ICollection<string> items)
        {
            var oldRecipient = SelectedRecipient;
            Recipients.Clear();
            Recipients.AddRange(items);
            Recipients.Add("All");
            SelectedRecipient = items.Contains(oldRecipient) ? oldRecipient : "All";
        }

        public void Handle(NewMessagesMessage msg)
        {
            foreach (var message in msg.Messages)
                Messages.Add(MessageMapper.Map(Settings, message));
        }
    }
}
