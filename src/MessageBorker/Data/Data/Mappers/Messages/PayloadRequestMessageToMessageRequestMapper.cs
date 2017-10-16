using Domain.Infrastructure.Mapping;
using Domain.Models;
using Messages.Payload;

namespace Data.Mappers.Messages
{
    public class PayloadRequestMessageToMessageRequestMapper : IMapper<PayloadRequestMessage, MessageRequest>
    {
        public MessageRequest Map(PayloadRequestMessage model)
        {
            return model == null ? null : new MessageRequest {QueueName = model.QueueName};
        }
    }
}