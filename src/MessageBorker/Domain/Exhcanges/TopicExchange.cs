using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Domain.Messages;

namespace Domain.Exhcanges
{
    public class TopicExchange : Exchange
    {
        public TopicExchange(string name) : base(name)
        {
        }

        public TopicExchange(string name, Dictionary<string, Queue<Message>> queues) : base(name, queues)
        {
        }

        protected override List<Queue<Message>> SelectQueues(Message message)
        {
            var routingRegex = new Regex(message.RoutingKey);
            return Queues.Where(queue => routingRegex.IsMatch(queue.Key))
                .Select(queue => queue.Value)
                .ToList();
        }
    }
}