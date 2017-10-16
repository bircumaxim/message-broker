using Data.Models;
using Domain.Infrastructure.Mapping;
using Persistence.Models;

namespace Data.Mappers.Persistence
{
    public class PersitenceServerGeneralInfoMapper : IMapper<PersistenceServerGeneralInfo, ServerGeneralInfo>
    {
        public ServerGeneralInfo Map(PersistenceServerGeneralInfo model)
        {
            return model == null
                ? null
                : new ServerGeneralInfo
                {
                    ServerStartTime = model.ServerStartTime,
                    MessagesInQueue = model.MessagesInQueue
                };
        }
    }
}