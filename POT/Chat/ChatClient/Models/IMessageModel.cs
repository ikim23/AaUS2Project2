using System;

namespace ChatClient.Models
{
    public interface IMessageModel
    {
        DateTime SendTime { get; set; }
        string Sender { get; set; }
        void UpdateStyle(SettingsModel settings);
    }
}
