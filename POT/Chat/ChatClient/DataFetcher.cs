using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Caliburn.Micro;
using ChatClient.Messages;
using ChatLibrary;

namespace ChatClient
{
    public class DataFetcher : IHandle<DisconnectMessage>, IHandle<SendMessage>
    {
        private readonly object _serviceLock = new object();
        private readonly IEventAggregator _aggregator;
        private readonly ChannelFactory<IChatService> _factory;
        private IChatService _service;
        private string _userId;
        private List<string> _onlineUsers = new List<string>();
        private DateTime _lastDate = DateTime.MinValue;

        public DataFetcher(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            aggregator.Subscribe(this);
            _factory = new ChannelFactory<IChatService>("Client");
            _service = _factory.CreateChannel();
        }

        public bool LogIn(string userId)
        {
            lock (_serviceLock)
            {
                if (!_service.LogIn(userId)) return false;
            }
            _userId = userId;
            FetchPeriodically();
            return true;
        }

        private void FetchPeriodically()
        {
            new Thread(() =>
            {
                while (_service != null)
                {
                    try
                    {
                        FetchOnlineUsers();
                        FetchNewMessages();
                        Thread.Sleep(1000);
                    }
                    catch (Exception)
                    {
                    }
                }
            }).Start();
        }

        private void FetchOnlineUsers()
        {
            List<string> updated;
            lock (_serviceLock)
            {
                updated = _service
                    .GetOnlineUsers()
                    .Where(userId => userId != _userId)
                    .ToList();
            }
            if (_onlineUsers.Count == updated.Count)
            {
                var areEqual = true;
                for (var i = 0; i < _onlineUsers.Count; i++)
                    if (!(areEqual = _onlineUsers[i] == updated[i])) break;
                if (areEqual) return;
            }
            _aggregator.PublishOnUIThread(new OnlineUsersMessage
            {
                OnlineUsers = _onlineUsers = updated
            });
        }

        private void FetchNewMessages()
        {
            List<IMessage> messages;
            lock (_serviceLock)
            {
                messages = _service.GetMessages(_userId, _lastDate, 30).ToList();
            }
            if (messages.Count <= 0) return;
            _lastDate = messages.Last().SendTime;
            _aggregator.PublishOnUIThread(new NewMessagesMessage
            {
                Messages = messages
            });
        }

        public void Handle(DisconnectMessage message)
        {
            lock (_serviceLock)
            {
                if (_userId != null)
                    _service.LogOut(_userId);
                _factory.Close();
                _service = null;
            }
        }

        public void Handle(SendMessage message)
        {
            lock (_serviceLock)
            {
                if (_userId == null || _service == null) return;
                message.Message.Sender = _userId;
                _service.SendMessage(message.Message);
            }
        }
    }
}
