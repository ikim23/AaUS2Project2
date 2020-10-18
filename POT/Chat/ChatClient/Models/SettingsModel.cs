using System.Windows.Media;

namespace ChatClient.Models
{
    public class SettingsModel
    {
        public int FontSize { get; set; } = 12;
        public Color TextColor { get; set; } = Colors.Black;
        public Color BackgroundColor { get; set; } = Colors.Yellow;
    }
}
