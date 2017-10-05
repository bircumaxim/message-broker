using System.Collections.Generic;
using System.Linq;
using Domain.Messages;

namespace Domain.Exhcanges
{
    public class FanoutExchange : Exchange
    {
        public FanoutExchange(string name) : base(name)
        {
        }

        public FanoutExchange(string name, Dictionary<string, Queue<Message>> queues) : base(name, queues)
        {
        }

        protected override List<Queue<Message>> SelectQueues(Message message)
        {
            return Queues.Select(queue => queue.Value).ToList();
        }
    }
}