using System.Collections.Generic;
using System.Linq;
using Domain.Messages;

namespace Domain.Exhcanges
{
    public class DefaultExchange : Exchange
    {
        public DefaultExchange(string name) : base(name)
        {
        }

        public DefaultExchange(string name, Dictionary<string, Queue<Message>> queues) : base(name, queues)
        {
        }

        protected override List<Queue<Message>> SelectQueues(Message message)
        {
            return Queues.Where(queue => queue.Value.Name == message.RoutingKey)
                .Select(queue => queue.Value)
                .ToList();
        }
    }
}