using System.Collections.Generic;
using System.Linq;
using Domain.Models;

namespace Domain.Exhcanges
{
    public class DefaultExchange : Exchange
    {
        public DefaultExchange(string name) : base(name)
        {
        }

        public DefaultExchange(string name, Dictionary<string, Queue<RouteMessage>> queues) : base(name, queues)
        {
        }

        protected override List<Queue<RouteMessage>> SelectQueues(RouteMessage routeMessage)
        {
            return Queues.Where(queue => queue.Value.Name == routeMessage.RoutingKey)
                .Select(queue => queue.Value)
                .ToList();
        }
    }
}