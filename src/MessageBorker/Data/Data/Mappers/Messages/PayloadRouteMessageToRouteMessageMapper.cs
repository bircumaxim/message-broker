using System.IO;
using Domain.Infrastructure.Mapping;
using Domain.Models;
using Messages.Payload;

namespace Data.Mappers.Messages
{
    public class PayloadRouteMessageToRouteMessageMapper : IMapper<PayloadRouteMessage, RouteMessage>
    {
        public RouteMessage Map(PayloadRouteMessage model)
        {
            if (model == null)
            {
                return null;
            }
            return new RouteMessage
            {
                MessageId = model.MessageId,
                ExchangeName = model.ExchangeName,
                IsDurable = model.IsDurable,
                RoutingKey = model.RoutingKey,
                Payload = model.Payload
            };
        }      
    }
}