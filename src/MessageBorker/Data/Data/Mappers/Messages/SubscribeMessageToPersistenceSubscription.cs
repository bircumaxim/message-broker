using Domain.Infrastructure.Mapping;
using Messages.Subscribe;
using Persistence.Models;

namespace Data.Mappers.Messages
{
    public class SubscribeMessageToPersistenceSubscription : IMapper<SubscribeMessage, PersistenceSubscription>
    {
        public PersistenceSubscription Map(SubscribeMessage model)
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