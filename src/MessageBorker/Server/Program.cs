using System;
using System.Collections.Generic;
using System.Net;
using Serialization.WireProtocols;
using Transport.Events;
using Transport.Tcp;
using Transport.Tcp.Events;
using Transport.UDP;

namespace Server
{
    internal class Program
    {
        private static readonly List<TcpConnector> Connections = new List<TcpConnector>();
        private const int Port = 5000;

        public static void Main(string[] args)
        {
//            var tcpConnectionListener = new TcpConnectionListener(Port);
//            tcpConnectionListener.TcpClientConnected += OnClientConnected;
//            tcpConnectionListener.StartAsync();
//
//            Console.WriteLine("Server started on port 5000");
//            Console.WriteLine("Press any keyt to spot the server");
//            Console.ReadKey();
//            tcpConnectionListener.Stop();
            
            
            
            var udpConnector = new UdpConnector(6000, 1, new DefaultWireProtocol());
            udpConnector.StartAsync();
            EndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);
            udpConnector.SendMessage(new HealthCheckMessage {SourceServerName = "Test server"}, endPoint);
            
            Console.WriteLine("Server is up !");
            Console.WriteLine("Press any key to shutdown");
            Console.ReadKey();
            
            
        }
        
        public static void OnConnectorStateChanged(object sender, ConnectorStateChangeEventArgs args)
        {
            if (args.NewState != ConnectionState.Connected) return;
            Console.WriteLine("Message sent!");
            args.ConnectionOrientedConnector.SendMessage(new HealthCheckMessage{SourceServerName = "Test server"});
        }

        public static void OnClientConnected(object sender, TcpClientConnectedEventArgs args)
        {
            var tcpConnector = new TcpConnector(args.ClientSocket, Guid.NewGuid().GetHashCode(), new DefaultWireProtocol());
            tcpConnector.MessageReceived += OnConnectorMessageReceived;
            tcpConnector.StartAsync();
            Connections.Add(tcpConnector);
        }

        public static void OnConnectorMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            if (args.Message.MessageTypeId != 1) return;
            var message = args.Message as HealthCheckMessage;
            Console.WriteLine($"Message form {args.Connector.ConnectorId} {message?.SourceServerName}");
        }
    }
}