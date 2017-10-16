using Domain.Infrastructure.Mapping;
using Domain.Models;
using Persistence.Models;

namespace Data.Mappers.Persistence
{
    public class PersistenceMessageMapper : IMapper<PersistenceMessage, Message>
    {
        public Message Map(PersistenceMessage model)
        {
            return model == null
                ? null
                : new Message
                {
                    MessageId = model.MessageId,
                    Payload = model.Payload
                };
        }
    }
}