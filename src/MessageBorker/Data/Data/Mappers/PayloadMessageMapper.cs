using System.IO;
using Data.Models;
using Domain.Infrastructure.Mapping;
using Messages.Payload;

namespace Data.Mappers
{
    public class PayloadMessageMapper : ITwoWaysMapper<PayloadMessage, MessageData>
    {
        public MessageData Map(PayloadMessage model)
        {
            if (model == null)
            {
                return null;
            }
            return new MessageData
            {
                MessageId = model.MessageId,
                ExchangeName = model.ExchangeName,
                IsDurable = model.IsDurable,
                RoutingKey = model.RoutingKey,
                Payload = model.Payload
            };
        }

        public PayloadMessage InverseMap(MessageData model)
        {
            if (model == null)
            {
                return null;
            }
            return new PayloadMessage
            {
                MessageId = model.MessageId,
                ExchangeName = model.ExchangeName,
                IsDurable = model.IsDurable,
                RoutingKey = model.RoutingKey,
                MemoryStream = new MemoryStream(model.Payload),
                Payload = model.Payload
            };
        }
    }
}