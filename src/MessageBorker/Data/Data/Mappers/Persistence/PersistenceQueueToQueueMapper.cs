using Domain;
using Domain.Infrastructure.Mapping;
using Domain.Models;
using Persistence.Models;

namespace Data.Mappers.Persistence
{
    public class PersistenceQueueToQueueMapper : IMapper<PersistenceQueue<RouteMessage>, Queue<RouteMessage>>
    {
        public Queue<RouteMessage> Map(PersistenceQueue<RouteMessage> model)
        {
            return new Queue<RouteMessage> {Name = model.Name};
        }
    }
}