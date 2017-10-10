using Domain.Infrastructure.Mapping;
using Domain.Messages;
using Messages;
using Messages.Payload;

namespace Data.Mappers
{
    public class PayloadRequestMapper : IMapper<PayloadRequest, MessageRequest>
    {
        public MessageRequest Map(PayloadRequest model)
        {
            return model == null ? null : new MessageRequest {QueueName = model.QueueName};
        }
    }
}