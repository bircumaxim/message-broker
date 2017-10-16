using Domain.Infrastructure.Mapping;
using Domain.Models;
using Messages.Payload;

namespace Data.Mappers.Messages
{
    public class MessageToPayloadMessageMapper : IMapper<Message, PayloadMessage>
    {
        public PayloadMessage Map(Message model)
        {
            return model == null
                ? null
                : new PayloadMessage
                {
                    MessageId = model.MessageId,
                    Payload = model.Payload
                };
        }
    }
}