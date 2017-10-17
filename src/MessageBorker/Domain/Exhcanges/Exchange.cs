using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using log4net;

namespace Domain.Exhcanges
{
    public abstract class Exchange
    {
        public string Name { get; }
        private readonly ILog _logger;
        public readonly Dictionary<string, Queue<RouteMessage>> Queues;

        protected Exchange(string name)
        {
            Name = name;
            Queues = new Dictionary<string, Queue<RouteMessage>>();
            _logger = LogManager.GetLogger(GetType());
            Validate();
        }

        protected Exchange(string name, Dictionary<string, Queue<RouteMessage>> queues)
        {
            Name = name;
            Queues = queues;
        }

        public void Route(RouteMessage routeMessage)
        {
            SelectQueues(routeMessage).ForEach(queue => queue.Enqueue(routeMessage));
        }

        protected abstract List<Queue<RouteMessage>> SelectQueues(RouteMessage routeMessage);

        public void AddQueue(string bindingKey, Queue<RouteMessage> queue)
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

        public List<Queue<RouteMessage>> GetAllQueues()
        {
            return Queues.Values.ToList();
        }
    }
}