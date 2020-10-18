using System;

namespace ChatLibrary
{
    public class ImageMessage : IMessage
    {
        public DateTime SendTime { get; set; } = DateTime.Now;
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public byte[] Image { get; set; }
        public override string ToString() => $"ImageMessage from: {Sender} to: {Receiver}";
    }
}
