using System;
using System.Net;
using System.Net.Sockets;
using Serialization.WireProtocols;
using Transport.Connectors.Tcp;

namespace Producer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(IPAddress.Parse("127.0.1"), 9000);
            var tcpConnector = new TcpConnector(socket, 1, new DefaultWireProtocol());
            tcpConnector.StartAsync();

            Console.WriteLine("Stop Producer");
            Console.ReadKey();
        }
    }
}