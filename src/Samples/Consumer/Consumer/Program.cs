using System;
using System.Net;
using System.Threading.Tasks;
using MessageBuss.Buss;
using MessageBuss.Buss.Events;
using Messages.Connection;
using Serialization.WireProtocol;
using Transport.Connectors.UdpMulticast;
using Transport.Connectors.UdpMulticast.Events;

namespace Consumer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var buss = BussFactory.Instance.GetBussFor("Broker2");
            buss.MessageReceived += OnMessageReceived;
            Console.ReadLine();

//            var udpMulticastBrocker = new UdpMulticastBrocker("test", new DefaultWireProtocol(false),
//                new IPEndPoint(IPAddress.Parse("224.5.6.7"), 7000), null);
//
//            udpMulticastBrocker.StartAsync();
//            udpMulticastBrocker.MessageReceivedFromBrokerHandler += OnMessageReceived2;
//            Console.ReadLine();

//            var openConnectionRequest = new OpenConnectionRequest();
//
//            var udpMulticastReceiver = new UdpMulticastReceiver(new IPEndPoint(IPAddress.Parse("224.5.6.7"), 7000), new DefaultWireProtocol());
//            udpMulticastReceiver.UdpMulticastMessageReceivedHandler += OnMessageReceived2;
//            Task.Factory.StartNew(udpMulticastReceiver.StartReceivingMessages);
//            Console.ReadKey();
        }

        private static void OnMessageReceived2(object sender, UdpMulticastMessageReceivedEventArgs args)
        {
            var userOrderPayload = args.Message as UserOrderPayload;
            Console.WriteLine(userOrderPayload?.ToString());
        }

        public static void OnMessageReceived(object sender, MessegeReceviedEventArgs args)
        {
           
        }
    }
}