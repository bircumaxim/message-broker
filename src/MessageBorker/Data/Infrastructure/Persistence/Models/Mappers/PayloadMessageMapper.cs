using System;
using Messages.Payload;

namespace Persistence.Models.Mappers
{
    public class PayloadMessageMapper
    {
        public PayloadMessage Map(PersistenceMessage model)
        {
            return model == null
                ? null
                : new PayloadMessage
                {
                    DestinationQueueName = model.DestinationQueueName,
                    TimeStamp = DateTime.Now,
                    MessageId = model.MessageId,
                    Payload = model.Payload
                };
        }
        public PersistenceMessage InversMap(PayloadMessage model)
        {
            return model == null
                ? null
                : new PersistenceMessage
                {
                    DestinationQueueName = model.DestinationQueueName,
                    MessageId = model.MessageId,
                    Payload = model.Payload
                };
        }
    }
}