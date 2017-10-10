using System;
using MessageBuss.Buss;
using MessageBuss.Buss.Events;
using Messages.ServerInfo;

namespace Producer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var buss = BussFactory.Instance.GetBussFor("Brocker");
            buss.Ping();
            buss.MessageReceived += OnMessageReceived;
            buss.Fanout(new UserOrderPayload());
            buss.RequestServerInfo();
            Console.ReadKey();
            buss.Dispose();
            Console.WriteLine("Producer was closed.\nPress any key to exit");
            Console.ReadLine();
        }

        private static void OnMessageReceived(object sender, MessegeReceviedEventArgs args)
        {
            
        }
    }
}