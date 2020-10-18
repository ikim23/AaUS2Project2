using System.Windows.Media;
using ChatLibrary;

namespace ChatClient.Models
{
    public class MessageMapper
    {
        public static IMessageModel Map(SettingsModel settings, IMessage message)
        {
            if (message is TextMessage)
            {
                var msg = (TextMessage)message;
                if (msg.Receiver != null)
                {
                    return new PrivateTextMessageModel
                    {
                        SendTime = msg.SendTime,
                        Sender = msg.Sender,
                        Receiver = msg.Receiver,
                        Message = msg.Message,
                        FontSize = settings.FontSize,
                        TextColor = new SolidColorBrush(settings.TextColor),
                        BackgroundColor = new SolidColorBrush(settings.BackgroundColor)
                    };
                }
                return new PublicTextMessageModel
                {
                    SendTime = msg.SendTime,
                    Sender = msg.Sender,
                    Message = msg.Message,
                    FontSize = settings.FontSize,
                    TextColor = new SolidColorBrush(settings.TextColor),
                };
            }
            if (message is ImageMessage)
            {
                var msg = (ImageMessage)message;
                if (msg.Receiver != null)
                {
                    return new PrivateImageMessageModel
                    {
                        SendTime = msg.SendTime,
                        Sender = msg.Sender,
                        Receiver = msg.Receiver,
                        Image = msg.Image,
                        BackgroundColor = new SolidColorBrush(settings.BackgroundColor)
                    };
                }
                return new PublicImageMessageModel
                {
                    SendTime = msg.SendTime,
                    Sender = msg.Sender,
                    Image = msg.Image
                };
            }
            return null;
        }
    }
}
