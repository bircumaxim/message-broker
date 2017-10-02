using Transport;
using Transport.Events;

namespace Data
{
    public interface IConnectionManager : IRun
    {
        event ConnectorConnectedHandler ConnectorConnected;
    }
}