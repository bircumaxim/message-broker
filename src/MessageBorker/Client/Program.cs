﻿using System;
using System.Net.Sockets;
using Serialization.WireProtocols;
using Transport.Events;
using Transport.Tcp;

namespace Client
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("127.0.0.1", 9000);
            var tcpConnector = new TcpConnector(socket, Guid.NewGuid().GetHashCode(), new DefaultWireProtocol());
            tcpConnector.StartAsync();
            tcpConnector.StateChanged += OnConnectorStateChanged;
            Console.WriteLine("Client was connected !");
            Console.WriteLine("Press any key to close connection");
            Console.ReadKey();
        }

        public static void OnConnectorStateChanged(object sender, ConnectorStateChangeEventArgs args)
        {
            if (args.NewState == ConnectionState.Connected)
            {
                args.Connector.SendMessage(new HealthCheckMessage{SourceServerName = "Test server"});                
            }
        }
    }
}