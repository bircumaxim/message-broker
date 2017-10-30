using System;
using System.Net.Sockets;
using MessageBuss.Buss;
using MessageBuss.Buss.Events;
using Messages.Connection;
using Messages.ServerInfo;

namespace Producer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var buss = BussFactory.Instance.GetBussFor("Broker");
            
            buss.Ping();
            buss.MessageReceived += OnMessageReceived;
            buss.Fanout(new UserOrderPayload {UserName = "Papusoi Ion"}, true);
            Console.ReadKey();
            buss.RequestServerInfo();

            var buss2 = BussFactory.Instance.GetBussFor("Broker2");
            buss2.MessageReceived += OnMessageReceived;
            buss2.Fanout(new UserOrderPayload(), true);
            Console.ReadKey();
            buss.Dispose();
            buss2.Dispose();
            Console.WriteLine("Producer was closed.\nPress any key to exit");
            Console.ReadLine();
        }

        private static void OnMessageReceived(object sender, MessegeReceviedEventArgs args)
        {
            if (args.Payload.MessageTypeName == typeof(PongMessage).Name)
            {
                Console.WriteLine("Server is alive!");
            }
            if (args.Payload.MessageTypeName == typeof(ServerGeneralInfoResponse).Name)
            {
                var serverInfo = args.Payload as ServerGeneralInfoResponse;
                Console.WriteLine($"Server start time: {serverInfo?.ServerStartTime}\n" +
                                  $"Connections count: {serverInfo?.ConnectionsCount}\n" +
                                  $"Messages in queue {serverInfo?.MessagesInQueue}");
            }
        }
    }
}