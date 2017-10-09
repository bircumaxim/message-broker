using Domain.Infrastructure.Mapping;
using Domain.Messages;
using Messages;

namespace Data.Mappers
{
    public class DefaultMessageRequestMapper : IMapper<DefaultMessageRequest, MessageRequest>
    {
        public MessageRequest Map(DefaultMessageRequest model)
        {
            return model == null ? null : new MessageRequest {QueueName = model.QueueName};
        }
    }
}