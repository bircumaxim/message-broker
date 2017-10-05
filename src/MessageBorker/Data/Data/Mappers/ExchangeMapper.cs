using System.Linq;
using Data.Models;
using Domain;
using Domain.Exhcanges;
using Domain.Infrastructure.Mapping;
using Domain.Messages;

namespace Data.Mappers
{
    public class ExchangeMapper : IMapper<ExchangeData, Exchange>
    {
        public Exchange Map(ExchangeData model)
        {
            Exchange exchange;
            switch (model.ExchangeDataType)
            {
                case ExchangeDataType.Direct:
                    exchange = new DirectExchange(model.Name);
                    break;
                case ExchangeDataType.Fanout:
                    exchange = new FanoutExchange(model.Name);
                    break;
                case ExchangeDataType.Topic:
                    exchange = new TopicExchange(model.Name);
                    break;
                default:
                    exchange = new DefaultExchange(model.Name);
                    break;
            }
            model.Queues.Keys.ToList().ForEach(key => exchange.AddQueue(key, new Queue<Message>()));
            return exchange;
        }
    }
}