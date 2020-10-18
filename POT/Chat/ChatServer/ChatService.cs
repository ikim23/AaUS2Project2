using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using ChatLibrary;

namespace ChatServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ChatService : IChatService
    {
        public static readonly Action<string> Log = msg => Console.WriteLine(msg);
        private readonly ISet<string> _onlineUsers = new SortedSet<string>();
        private readonly IList<IMessage> _messages = new List<IMessage>();

        public bool LogIn(string userId)
        {
            var result = _onlineUsers.Add(userId);
            Log($"LogIn - name: {userId} success: {result}");
            return result;
        }

        public bool LogOut(string userId)
        {
            var result = _onlineUsers.Remove(userId);
            Log($"LogOut - name: {userId} success: {result}");
            return result;
        }

        public IEnumerable<string> GetOnlineUsers()
        {
            return _onlineUsers;
        }

        public bool SendMessage(IMessage message)
        {
            Log($"SendMessage - {message}");
            _messages.Add(message);
            return true;
        }

        public IEnumerable<IMessage> GetMessages(string userId, DateTime fromDate, int count)
        {
            return _messages
                .Where(m => BelongsTo(m, userId))
                .Where(m => m.SendTime > fromDate)
                .OrderBy(m => m.SendTime)
                .Take(count);
        }

        private bool BelongsTo(IMessage msg, string userId)
        {
            if (msg.Sender == userId) return true;
            if (msg.Receiver == userId) return true;
            if (msg.Receiver == null) return true;
            return false;
        }
    }
}
