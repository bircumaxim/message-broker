using System.Net;
using Serialization;
using Transport.Events;

namespace Transport
{
    public interface IConnector : IRun
    {     
        event MessageReceivedHandler MessageReceived;

        long ConnectorId { get;  }
    }
    
    public interface IConnectionOrientedConnector 
    {
        event ConnectorStateChangeHandler StateChanged;
        
        CommunicationWay CommunicationWay { get; set; }

        void SendMessage(Message message);
    }

    public interface IConnectionLessConnector 
    {
        void SendMessage(Message message, EndPoint endPoint);
    }
}