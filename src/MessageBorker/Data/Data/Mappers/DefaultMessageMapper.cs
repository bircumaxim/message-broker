using Data.Models;
using Domain.Infrastructure.Mapping;
using Domain.Messages;
using Messages;


namespace Data.Mappers
{
    public class DefaultMessageMapper : IMapper<DefaultMessage, Message>
    {
        public Message Map(DefaultMessage model)
        {
            if (model == null)
            {
                return null;
            }
            return new Message
            {
                ExchangeName = model.ExchangeName,
                IsDurable = model.IsDurable,
                RoutingKey = model.RoutingKey,
                Payload = model.Payload
            };
        }
    }
}