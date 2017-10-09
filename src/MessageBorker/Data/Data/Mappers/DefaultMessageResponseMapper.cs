using Domain.Infrastructure.Mapping;
using Domain.Messages;
using Messages;

namespace Data.Mappers
{
    public class DefaultMessageResponseMapper : IMapper<MessageResponse, DefaultMessageResponse>
    {
        public DefaultMessageResponse Map(MessageResponse model)
        {
            return model == null ? null : new DefaultMessageResponse {Payload = model.Payload};
        }
    }
}