using Data.Mappers;
using Data.Models;
using Messages.ServerInfo;

namespace Data.Commands
{
    public class SendServerGeneralInfoCommand : ICommand
    {
        private readonly string _applicationName;
        private readonly Persistence _persistence;
        private readonly RemoteApplicationManager _remoteApplicationManager;

        public SendServerGeneralInfoCommand(string applicationName, RemoteApplicationManager remoteApplicationManager,
            Persistence persistence)
        {
            _applicationName = applicationName;
            _persistence = persistence;
            _remoteApplicationManager = remoteApplicationManager;
        }

        public void Execute()
        {
            var serverGeneralInfo = _persistence.GetServerInfo();
            serverGeneralInfo.ConnectionsCount = _remoteApplicationManager.GetConnectionsNumber();
            var serverGeneralInfoResponse = MappersPull.Instance.Map<ServerGeneralInfo, ServerGeneralInfoResponse>(serverGeneralInfo);
            _remoteApplicationManager.SendMessage(_applicationName, serverGeneralInfoResponse);
        }
    }
}