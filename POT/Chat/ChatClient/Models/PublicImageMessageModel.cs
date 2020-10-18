using System;

namespace ChatClient.Models
{
   public class PublicImageMessageModel : IMessageModel
    {
        public DateTime SendTime { get; set; }
        public string Sender { get; set; }
        public byte[] Image { get; set; }
        public int FontSize { get; set; }

        public void UpdateStyle(SettingsModel settings)
        {
            FontSize = settings.FontSize;
        }
    }
}
