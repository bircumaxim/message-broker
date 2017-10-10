using System.Linq;
using Domain;
using Domain.Exhcanges;
using Domain.Infrastructure.Mapping;
using Domain.Messages;

namespace Data.Models.Mappers
{
    public class ExchangeMapper : IMapper<ExchangeData, Exchange>
    {
        public Exchange Map(ExchangeData model)
        {
            if (model == null)
            {
                return null;
            }
            Exchange exchange;
            switch (model.ExchangeTypeData)
            {
                case ExchangeTypeData.Direct:
                    exchange = new DirectExchange(model.Name);
                    break;
                case ExchangeTypeData.Fanout:
                    exchange = new FanoutExchange(model.Name);
                    break;
                case ExchangeTypeData.Topic:
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