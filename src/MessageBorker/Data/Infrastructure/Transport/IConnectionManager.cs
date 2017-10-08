using Transport.Events;

namespace Transport
{
    public interface IConnectionManager : IRun
    {
        event ConnectorConnectedHandler ConnectorConnected;
    }
}