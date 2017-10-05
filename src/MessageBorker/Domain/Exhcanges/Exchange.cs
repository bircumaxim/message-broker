using System;
using System.Collections.Generic;
using Domain.Messages;
using log4net;

namespace Domain.Exhcanges
{
    public abstract class Exchange
    {
        public string Name { get; }
        private readonly ILog _logger;
        protected readonly Dictionary<string, Queue<Message>> Queues;

        protected Exchange(string name)
        {
            Name = name;
            Queues = new Dictionary<string, Queue<Message>>();
            _logger = LogManager.GetLogger(GetType());
            Validate();
        }

        protected Exchange(string name, Dictionary<string, Queue<Message>> queues)
        {
            Name = name;
            Queues = queues;
        }

        public void Route(Message message)
        {
            SelectQueues(message).ForEach(queue => queue.Enqueue(message));
        }

        protected abstract List<Queue<Message>> SelectQueues(Message message);

        public void AddQueue(string bindingKey, Queue<Message> queue)
        {
            Queues.Add(bindingKey, queue);
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                _logger.Error("Name is required for an exhange !");
                throw new NullReferenceException();
            }
        }
    }
}