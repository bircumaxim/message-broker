using System.Collections.Generic;
using Domain.Exhcanges;
using Domain.Models;

namespace Domain.GateWays
{
    public interface IPersistenceGateWay
    {
        void PersistQueues(Dictionary<string, Queue<RouteMessage>> queues);
        Exchange GetExchangeFor(RouteMessage routeMessage);
        Message GetMessageFromQueueWithName(string queueName);
    }
}