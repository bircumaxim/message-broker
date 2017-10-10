using Serialization;
using Transport.Events;

namespace Transport.Connectors
{
    public interface IConnector : IRun
    {     
        event MessageReceivedHandler MessageReceived;

        string ConnectorId { get;  }
        
        void SendMessage(Message message);
    }
    
    public interface IConnectionOrientedConnector 
    {
        event ConnectorStateChangeHandler StateChanged;
        
        CommunicationWay CommunicationWay { get; set; }
    }
}