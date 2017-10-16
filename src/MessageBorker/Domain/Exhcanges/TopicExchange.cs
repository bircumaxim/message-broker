using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Domain.Models;

namespace Domain.Exhcanges
{
    public class TopicExchange : Exchange
    {
        public TopicExchange(string name) : base(name)
        {
        }

        public TopicExchange(string name, Dictionary<string, Queue<RouteMessage>> queues) : base(name, queues)
        {
        }

        protected override List<Queue<RouteMessage>> SelectQueues(RouteMessage routeMessage)
        {
            var routingRegex = new Regex(routeMessage.RoutingKey);
            return Queues.Where(queue => routingRegex.IsMatch(queue.Key))
                .Select(queue => queue.Value)
                .ToList();
        }
    }
}