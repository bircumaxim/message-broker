using System.Linq;
using Domain;
using Domain.Exhcanges;
using Domain.Infrastructure.Mapping;
using Domain.Models;
using Persistence.Models;

namespace Data.Mappers.Persistence
{
    public class PersistenceExchangeToExchangeMapper : IMapper<PersistenceExchange, Exchange>
    {
        public Exchange Map(PersistenceExchange model)
        {
            if (model == null)
            {
                return null;
            }
            Exchange exchange;
            switch (model.ExchangeType)
            {
                case PersistenceExchangeType.Direct:
                    exchange = new DirectExchange(model.Name);
                    break;
                case PersistenceExchangeType.Fanout:
                    exchange = new FanoutExchange(model.Name);
                    break;
                case PersistenceExchangeType.Topic:
                    exchange = new TopicExchange(model.Name);
                    break;
                default:
                    exchange = new DefaultExchange(model.Name);
                    break;
            }
            model.Queues.Keys.ToList().ForEach(key => exchange.AddQueue(key, new Queue<RouteMessage>()));
            return exchange;
        }
    }
}