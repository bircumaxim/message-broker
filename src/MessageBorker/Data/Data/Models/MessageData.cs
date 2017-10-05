using System;
using System.ComponentModel;
using log4net;

namespace Data.Models
{
    public class MessageData
    {
        public bool IsDurable { get; set; }
        public string RoutingKey { get; set; }
        public string ExchangeName { get; set; }
        public byte[] Payload { get; set; }
    }
}