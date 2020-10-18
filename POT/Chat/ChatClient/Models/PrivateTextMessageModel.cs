using System;
using System.Windows.Media;

namespace ChatClient.Models
{
    public class PrivateTextMessageModel : IMessageModel
    {
        public DateTime SendTime { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Message { get; set; }
        public int FontSize { get; set; }
        public Brush TextColor { get; set; }
        public Brush BackgroundColor { get; set; }

        public void UpdateStyle(SettingsModel settings)
        {
            FontSize = settings.FontSize;
            TextColor = new SolidColorBrush(settings.TextColor);
            BackgroundColor = new SolidColorBrush(settings.BackgroundColor);
        }
    }
}
