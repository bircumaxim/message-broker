using Domain.Infrastructure.Mapping;
using Domain.Messages;

namespace Data.Models.Mappers
{
    public class MessageMapper : ITwoWaysMapper<MessageData, Message>
    {
        public Message Map(MessageData model)
        {
            return new Message
            {
                MessageId = model.MessageId,
                ExchangeName = model.ExchangeName,
                IsDurable = model.IsDurable,
                RoutingKey = model.RoutingKey,
                Payload = model.Payload
            };
        }

        public MessageData InverseMap(Message model)
        {
            return new MessageData
            {
                MessageId = model.MessageId,
                ExchangeName = model.ExchangeName,
                IsDurable = model.IsDurable,
                RoutingKey = model.RoutingKey,
                Payload = model.Payload
            };
        }
    }
}