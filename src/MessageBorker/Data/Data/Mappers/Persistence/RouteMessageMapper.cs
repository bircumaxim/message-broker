using Domain.Infrastructure.Mapping;
using Domain.Models;
using Persistence.Models;

namespace Data.Mappers.Persistence
{
    public class RouteMessageMapper : IMapper<RouteMessage, PersistenceMessage>
    {
        public PersistenceMessage Map(RouteMessage model)
        {
            return model == null
                ? null
                : new PersistenceMessage
                {
                    MessageId = model.MessageId,
                    Payload = model.Payload
                };
        }
    }
}