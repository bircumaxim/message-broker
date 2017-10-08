using System;
using System.Collections.Generic;
using System.Xml;
using Data.Models;

namespace Data.Configuration.FileConfiguration
{
    public class ExchangeAndQueuesConfiguration
    {
        private const string DefaultExchangeNme = "DefaultExchange";
        private const string DefaultQueuename = "DefaultQueue";

        public List<ExchangeData> Exchanges { get; }
        public Dictionary<string, QueueData<MessageData>> Queues { get; }

        public ExchangeAndQueuesConfiguration(XmlNode configsDocument)
        {
            Exchanges = new List<ExchangeData>();
            Queues = new Dictionary<string, QueueData<MessageData>>();
            LoadConfigurations(configsDocument);
        }

        private void LoadConfigurations(XmlNode configsDocument)
        {
            var exchangeNodes = configsDocument.SelectSingleNode("/MessageBrocker/Exchanges");
            if (exchangeNodes != null)
            {
                foreach (XmlElement exchangeNode in exchangeNodes)
                {
                    Exchanges.Add(CreateExchange(exchangeNode));
                }
            }
        }

        private ExchangeData CreateExchange(XmlNode exchangeNode)
        {
            var exchange = new ExchangeData();
            if (exchangeNode.Attributes != null)
            {
                exchange.Name = exchangeNode.Attributes.GetNamedItem("Name").Value ?? DefaultExchangeNme;
                switch (exchangeNode.Name)
                {
                    case "DirectExchange":
                        exchange.ExchangeDataType = ExchangeDataType.Direct;
                        break;
                    case "TopicExchange":
                        exchange.ExchangeDataType = ExchangeDataType.Topic;
                        break;
                    case "FanoutExchange":
                        exchange.ExchangeDataType = ExchangeDataType.Fanout;
                        break;
                }

                exchange.Queues = GetExchangeQueues(exchangeNode);
            }

            return exchange;
        }

        private Dictionary<string, QueueData<MessageData>> GetExchangeQueues(XmlNode exchangeNode)
        {
            var queuesThatBelongToGivenExchange = new Dictionary<string, QueueData<MessageData>>();
            foreach (XmlNode queueNode in exchangeNode.ChildNodes)
            {
                var queue = new QueueData<MessageData>();
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

        private void AddIfNotExist(QueueData<MessageData> queue)
        {
            if (!Queues.ContainsKey(queue.Name))
            {
                Queues.Add(queue.Name, queue);
            }
        }
    }
}