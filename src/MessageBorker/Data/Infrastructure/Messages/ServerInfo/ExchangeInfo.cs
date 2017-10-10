using Serialization;

namespace Messages.ServerInfo
{
    public class ExchangeInfo : Message
    {
        public string ExchangeName { get; set; }
    }
}