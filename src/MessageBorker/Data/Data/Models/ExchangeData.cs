using System.Collections.Generic;

namespace Data.Models
{
    public class ExchangeData
    {
        public string Name { get; set; }
        public ExchangeDataType ExchangeDataType { get; set; }
        public Dictionary<string, QueueData<MessageData>> Queues { get; set; }
    }
}