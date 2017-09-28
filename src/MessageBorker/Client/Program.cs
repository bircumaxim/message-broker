using System;
using System.Net.Sockets;
using Serialization;
using Serialization.WireProtocols;
using Transport.Events;
using Transport.Tcp;
using Transport.UDP;

namespace Client
{
    internal class Program
    {
        public static void Main(string[] args)
        {
//            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//            socket.Connect("127.0.0.1", 5000);
//            var tcpConnector = new TcpConnector(socket, Guid.NewGuid().GetHashCode(), new DefaultWireProtocol());
//            tcpConnector.StateChanged += OnConnectorStateChanged;
//            tcpConnector.StartAsync();
//            Console.WriteLine("Client was connected !");
//            Console.WriteLine("Press any key to close connection");
//            Console.ReadKey();


            var udpConnector = new UdpConnector(5000, 1, new DefaultWireProtocol());
            udpConnector.MessageReceived += OnMessageReceived;
            udpConnector.StartAsync();

            Console.WriteLine("Client is up !");
            Console.WriteLine("Press any key to shutdown");
            Console.ReadKey();
        }

        public static void OnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            if (args.Message.MessageTypeId != 1) return;
            var message = args.Message as HealthCheckMessage;
            Console.WriteLine($"Message form {args.Connector.ConnectorId} {message?.SourceServerName}");
        }

        public static void OnConnectorStateChanged(object sender, ConnectorStateChangeEventArgs args)
        {
            if (args.NewState == ConnectionState.Connected)
            {
                Console.WriteLine("Message sent!");
                args.ConnectionOrientedConnector.SendMessage(new HealthCheckMessage{SourceServerName = "Test server"});                
            }
        }
    }
}