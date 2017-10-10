using Domain.Infrastructure.Mapping;
using Domain.Messages;
using Messages.Payload;


namespace Data.Mappers
{
    public class PayloadMessageMapper : IMapper<PayloadMessage, Message>
    {
        public Message Map(PayloadMessage model)
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