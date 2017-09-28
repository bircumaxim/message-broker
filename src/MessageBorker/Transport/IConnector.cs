using Transport.Events;
using Transport.Serialization;

namespace Transport
{
    public interface IConnector
    {
        event ConnectorStateChangeHandler StateChanged;
        
        event MessageReceivedHandler MessageReceived;

        long ConnectorId { get;  }

        ConnectionState ConnectionState { get; }

        CommunicationWay CommunicationWay { get; set; }

        void SendMessage(Message message);
    }
}