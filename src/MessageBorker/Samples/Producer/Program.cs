using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Serialization.WireProtocols;
using Transport.Connectors.Tcp;
using Transport.Events;
using ConnectionState = Transport.Events.ConnectionState;

namespace Producer
{
    internal class Program
    {
        private static TcpConnector _tcpConnector;

        public static void Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(IPAddress.Parse("127.0.1"), 9000);
            _tcpConnector = new TcpConnector(socket, 1, new DefaultWireProtocol());
            _tcpConnector.StartAsync();
            _tcpConnector.StateChanged += OnStateChange;
            while (true)
            {
                
            }
        }

        public static void OnStateChange(object sender, ConnectorStateChangeEventArgs args)
        {
            if (args.NewState == ConnectionState.Connected)
            {
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Console.Write("Input message: ");
                        var payload = Console.ReadLine();
                        _tcpConnector.SendMessage(new PingMessage());
                    }
                });
            }
        }
    }
}