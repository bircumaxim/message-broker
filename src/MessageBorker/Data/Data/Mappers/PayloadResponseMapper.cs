using Domain.Infrastructure.Mapping;
using Domain.Messages;
using Messages;
using Messages.Payload;

namespace Data.Mappers
{
    public class PayloadResponseMapper : IMapper<MessageResponse, DefaultMessageResponse>
    {
        public DefaultMessageResponse Map(MessageResponse model)
        {
            return model == null ? null : new DefaultMessageResponse
            {
                ReceivedMessageId = model.MessageId,
                Payload = model.Payload
            };
        }
    }
}