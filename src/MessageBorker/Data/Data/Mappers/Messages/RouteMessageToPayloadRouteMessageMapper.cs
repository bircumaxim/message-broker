using System.IO;
using Domain.Infrastructure.Mapping;
using Domain.Models;
using Messages.Payload;

namespace Data.Mappers.Messages
{
    public class RouteMessageToPayloadRouteMessageMapper : IMapper<RouteMessage, PayloadRouteMessage>
    {
        public PayloadRouteMessage Map(RouteMessage model)
        {
            if (model == null)
            {
                return null;
            }
            return new PayloadRouteMessage
            {
                MessageId = model.MessageId,
                ExchangeName = model.ExchangeName,
                IsDurable = model.IsDurable,
                RoutingKey = model.RoutingKey,
                MemoryStream = new MemoryStream(model.Payload),
                Payload = model.Payload
            };
        }
    }
}