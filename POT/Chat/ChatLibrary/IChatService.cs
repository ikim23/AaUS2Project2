using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace ChatLibrary
{
    [ServiceContract]
    [ServiceKnownType(typeof(TextMessage))]
    [ServiceKnownType(typeof(ImageMessage))]
    public interface IChatService
    {
        [OperationContract]
        bool LogIn(string userId);
        [OperationContract]
        bool LogOut(string userId);
        [OperationContract]
        IEnumerable<string> GetOnlineUsers();
        [OperationContract]
        bool SendMessage(IMessage message);
        [OperationContract]
        IEnumerable<IMessage> GetMessages(string userId, DateTime fromDate, int count);
    }
}
