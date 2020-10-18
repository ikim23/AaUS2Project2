using System;

namespace ChatLibrary
{
    public interface IMessage
    {
        DateTime SendTime { get; set; }
        string Sender { get; set; }
        string Receiver { get; set; }
    }
}
