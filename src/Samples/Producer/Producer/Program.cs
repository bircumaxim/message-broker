using System;
using System.Net.Sockets;
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
            buss.Fanout(new UserOrderPayload {UserName = "Geaghea Vania"}, true);
            buss.RequestServerInfo();
            
//            var buss2 = BussFactory.Instance.GetBussFor("Brocker2");
//            buss2.Ping();
//            buss2.MessageReceived += OnMessageReceived;
//            buss2.Fanout(new UserOrderPayload(), true);
//            buss2.RequestServerInfo();
            Console.ReadKey();
            buss.Dispose();
//            buss2.Dispose();
            Console.WriteLine("Producer was closed.\nPress any key to exit");
            Console.ReadLine();
        }

        private static void OnMessageReceived(object sender, MessegeReceviedEventArgs args)
        {
            
        }
    }
}