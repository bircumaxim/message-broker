using Data.Models;
using Domain.Infrastructure.Mapping;
using Messages.ServerInfo;

namespace Data.Mappers.Messages
{
    public class ServerGeneralInfoResponseMapper : IMapper<ServerGeneralInfo, ServerGeneralInfoResponse>
    {
        public ServerGeneralInfoResponse Map(ServerGeneralInfo model)
        {
            return model == null ? null : new ServerGeneralInfoResponse
            {
                ServerStartTime = model.ServerStartTime,
                ConnectionsCount = model.ConnectionsCount
            };
        }
    }
}