using System;

namespace ChatLibrary
{
    public class TextMessage : IMessage
    {
        public DateTime SendTime { get; set; } = DateTime.Now;
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Message { get; set; }
        public override string ToString() => $"TextMessage from: {Sender} to: {Receiver} text: {Message}";
    }
}
