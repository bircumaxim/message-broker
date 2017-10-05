using System.Net.Sockets;

namespace Transport.Connectors.Tcp.Events
{
    public delegate void TcpClientConnectedHandler(object sender, TcpClientConnectedEventArgs args);

    public class TcpClientConnectedEventArgs
    {
        public Socket ClientSocket { get; set; }
    }
}