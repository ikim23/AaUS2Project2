using System;
using System.ServiceModel;

namespace ChatServer
{
    public class Program
    {
        public static void Main()
        {
            using (var host = new ServiceHost(typeof(ChatService)))
            {
                host.Open();
                Console.WriteLine("Server listening on port 5000...");
                Console.ReadLine();
                host.Close();
            }
        }
    }
}
