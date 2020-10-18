using System;
using System.Windows.Media;

namespace ChatClient.Models
{
    public class PrivateImageMessageModel : IMessageModel
    {
        public DateTime SendTime { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public byte[] Image { get; set; }
        public int FontSize { get; set; }
        public Brush BackgroundColor { get; set; }

        public void UpdateStyle(SettingsModel settings)
        {
            FontSize = settings.FontSize;
            BackgroundColor = new SolidColorBrush(settings.BackgroundColor);
        }
    }
}
