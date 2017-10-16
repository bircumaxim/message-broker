using System.Collections.Generic;
using System.Linq;
using Domain.Models;

namespace Domain.Exhcanges
{
    public class DirectExchange : Exchange
    {
        public DirectExchange(string name) : base(name)
        {
        }

        public DirectExchange(string name, Dictionary<string, Queue<RouteMessage>> queues) : base(name, queues)
        {
        }
        
        protected override List<Queue<RouteMessage>> SelectQueues(RouteMessage routeMessage)
        {
            return Queues.Where(queue => queue.Key == routeMessage.RoutingKey)
                .Select(queue => queue.Value)
                .ToList();
        }
    }
}