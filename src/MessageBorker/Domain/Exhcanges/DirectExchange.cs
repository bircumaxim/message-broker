using System.Collections.Generic;
using System.Linq;
using Domain.Messages;

namespace Domain.Exhcanges
{
    public class DirectExchange : Exchange
    {
        public DirectExchange(string name) : base(name)
        {
        }

        public DirectExchange(string name, Dictionary<string, Queue<Message>> queues) : base(name, queues)
        {
        }
        
        protected override List<Queue<Message>> SelectQueues(Message message)
        {
            return Queues.Where(queue => queue.Key == message.RoutingKey)
                .Select(queue => queue.Value)
                .ToList();
        }
    }
}