using Messages;
using Messages.Subscribe;

namespace Persistence.Models.Mappers
{
    public class SubscriptionMessageMapper
    {
        public SubscribeMessage Map(PersistenceSubscription model)
        {
            return model == null
                ? null
                : new SubscribeMessage
                {
                    Ip = model.Ip,
                    Port = model.Port,
                    QueueName = model.QueueName
                };
        }
        public PersistenceSubscription InversMap(SubscribeMessage model)
        {
            return model == null
                ? null
                : new PersistenceSubscription
                {
                    Ip = model.Ip,
                    Port = model.Port,
                    QueueName = model.QueueName
                };
        }
    }
}