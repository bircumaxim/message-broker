using System;
using System.Collections.Generic;
using System.Xml;
using Persistence.Models;

namespace Data.Configuration.FileConfiguration
{
    public class ExchangeAndQueuesConfiguration
    {
        private const string DefaultExchangeNme = "DefaultExchange";
        private const string DefaultQueuename = "DefaultQueue";

        public List<PersistenceExchange> Exchanges { get; }
        public Dictionary<string, PersistenceQueue<PersistenceMessage>> Queues { get; }

        public ExchangeAndQueuesConfiguration(XmlNode configsDocument)
        {
            Exchanges = new List<PersistenceExchange>();
            Queues = new Dictionary<string, PersistenceQueue<PersistenceMessage>>();
            LoadConfigurations(configsDocument);
        }

        private void LoadConfigurations(XmlNode configsDocument)
        {
            var exchangeNodes = configsDocument.SelectSingleNode("/Broker/Exchanges");
            if (exchangeNodes != null)
            {
                foreach (XmlElement exchangeNode in exchangeNodes)
                {
                    Exchanges.Add(CreateExchange(exchangeNode));
                }
            }
        }

        private PersistenceExchange CreateExchange(XmlNode exchangeNode)
        {
            var exchange = new PersistenceExchange();
            if (exchangeNode.Attributes != null)
            {
                exchange.Name = exchangeNode.Attributes.GetNamedItem("Name").Value ?? DefaultExchangeNme;
                switch (exchangeNode.Name)
                {
                    case "DirectExchange":
                        exchange.ExchangeType = PersistenceExchangeType.Direct;
                        break;
                    case "TopicExchange":
                        exchange.ExchangeType = PersistenceExchangeType.Topic;
                        break;
                    case "FanoutExchange":
                        exchange.ExchangeType = PersistenceExchangeType.Fanout;
                        break;
                }

                exchange.Queues = GetExchangeQueues(exchangeNode);
            }

            return exchange;
        }

        private Dictionary<string, PersistenceQueue<PersistenceMessage>> GetExchangeQueues(XmlNode exchangeNode)
        {
            var queuesThatBelongToGivenExchange = new Dictionary<string, PersistenceQueue<PersistenceMessage>>();
            foreach (XmlNode queueNode in exchangeNode.ChildNodes)
            {
                var queue = new PersistenceQueue<PersistenceMessage>();
                if (queueNode.Attributes != null)
                {
                    queue.Name = queueNode.Attributes.GetNamedItem("Name").Value ?? DefaultQueuename;
                }
                AddIfNotExist(queue);
                if (queuesThatBelongToGivenExchange.ContainsKey(queue.Name))
                {
                    throw new Exception($"A queue with Name: {queue.Name} is already binded to this exchange");
                }
                queuesThatBelongToGivenExchange.Add(queue.Name, queue);
            }
            return queuesThatBelongToGivenExchange;
        }

        private void AddIfNotExist(PersistenceQueue<PersistenceMessage> queue)
        {
            if (!Queues.ContainsKey(queue.Name))
            {
                Queues.Add(queue.Name, queue);
            }
        }
    }
}