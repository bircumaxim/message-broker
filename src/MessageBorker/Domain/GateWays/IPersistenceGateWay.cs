using System.Collections.Generic;
using Domain.Exhcanges;
using Domain.Messages;

namespace Domain.GateWays
{
    public interface IPersistenceGateWay
    {
        void PersistQueues(Dictionary<string, Queue<Message>> queues);
        Exchange GetExchangeFor(Message message);
    }
}