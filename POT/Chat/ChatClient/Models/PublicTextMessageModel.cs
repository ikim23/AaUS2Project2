using System;
using System.Windows.Media;

namespace ChatClient.Models
{
    public class PublicTextMessageModel : IMessageModel
    {
        public DateTime SendTime { get; set; }
        public string Sender { get; set; }
        public string Message { get; set; }
        public int FontSize { get; set; }
        public Brush TextColor { get; set; }

        public void UpdateStyle(SettingsModel settings)
        {
            FontSize = settings.FontSize;
            TextColor = new SolidColorBrush(settings.TextColor);
        }
    }
}
