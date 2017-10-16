using System.Collections.Generic;
using System.Linq;
using Domain.Models;

namespace Domain.Exhcanges
{
    public class FanoutExchange : Exchange
    {
        public FanoutExchange(string name) : base(name)
        {
        }

        public FanoutExchange(string name, Dictionary<string, Queue<RouteMessage>> queues) : base(name, queues)
        {
        }

        protected override List<Queue<RouteMessage>> SelectQueues(RouteMessage routeMessage)
        {
            return Queues.Select(queue => queue.Value).ToList();
        }
    }
}