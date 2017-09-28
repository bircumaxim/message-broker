using System;
using System.Collections.Generic;
using Serialization.WireProtocols;
using Transport.Events;
using Transport.Tcp;
using Transport.Tcp.Events;

namespace Server
{
    internal class Program
    {
        private static readonly List<TcpConnector> Connections = new List<TcpConnector>();
        private const int Port = 5000;

        public static void Main(string[] args)
        {
            var tcpConnectionListener = new TcpConnectionListener(Port);
            tcpConnectionListener.TcpClientConnected += OnClientConnected;
            tcpConnectionListener.StartAsync();

            Console.WriteLine("Server started on port 5000");
            Console.WriteLine("Press any keyt to spot the server");
            Console.ReadKey();
            tcpConnectionListener.Stop();
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