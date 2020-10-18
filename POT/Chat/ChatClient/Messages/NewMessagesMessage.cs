using System.Collections.Generic;
using ChatLibrary;

namespace ChatClient.Messages
{
    public class NewMessagesMessage
    {
        public List<IMessage> Messages { get; set; }
    }
}
