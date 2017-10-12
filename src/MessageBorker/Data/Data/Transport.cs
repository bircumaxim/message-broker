using System.Threading.Tasks;
using Data.Configuration;
using Data.Events;
using Data.Mappers;
using Domain.GateWays;
using Domain.Messages;
using Messages;
using Messages.Connection;
using Messages.ServerInfo;

namespace Data
{
    public class Transport : ITransportGateWay
    {
        private readonly ServerGeneralInfoResponseMapper _serverGeneralInfoResponseMapper;
        private readonly PayloadResponseMapper _payloadResponseMapper;
        private readonly RemoteApplicationManager _remoteApplicationManager;
        private readonly UseCaseFactory _useCaseFactory;
        private readonly Persistence _persistence;

        public Transport(IConfiguration configuration)
        {
            _persistence = new Persistence(configuration);
            _useCaseFactory = new UseCaseFactory(_persistence, this);
            _remoteApplicationManager = new RemoteApplicationManager(configuration);
            _remoteApplicationManager.RemoteApplicationMessageReceived += RemoteApplicationMessageReceived;
            _payloadResponseMapper = new PayloadResponseMapper();
            _serverGeneralInfoResponseMapper = new ServerGeneralInfoResponseMapper();
        }

        public void Start()
        {
            _persistence.GetPerisistedMessages().ForEach(message =>
            {
                RemoteApplicationMessageReceived(this,
                    new RemoteApplicationMessageReceivedEventArgs {Message = message});
            });
            _remoteApplicationManager.Start();
        }

        public Task StartAsync()
        {
            _persistence.GetPerisistedMessages().ForEach(message =>
            {
                RemoteApplicationMessageReceived(this,
                    new RemoteApplicationMessageReceivedEventArgs {Message = message});
            });
            return _remoteApplicationManager.StartAsync();
        }

        public void Stop()
        {
            _remoteApplicationManager.RemoteApplicationMessageReceived -= RemoteApplicationMessageReceived;
            _remoteApplicationManager.Stop();
        }

        private void RemoteApplicationMessageReceived(object seneder, RemoteApplicationMessageReceivedEventArgs args)
        {
            if (args.Message.MessageTypeName == typeof(CloseConnectionRequest).Name)
            {
                _remoteApplicationManager.StopRemoteApplication(args.Application.Name);
            }
            else if (args.Message.MessageTypeName == typeof(PingMessage).Name)
            {
                _remoteApplicationManager.SendMessage(args.Application.Name, new PongMessage());
            }
            else if (args.Message.MessageTypeName == typeof(ServerGerneralInfoRequest).Name)
            {
                var serverGeneralInfo = _persistence.GetServerInfo();
                serverGeneralInfo.ConnectionsCount = _remoteApplicationManager.GetConnectionsNumber();
                var serverGeneralInfoResponse = _serverGeneralInfoResponseMapper.Map(serverGeneralInfo);
                _remoteApplicationManager.SendMessage(args.Application.Name, serverGeneralInfoResponse);
            }
            else if (args.Message.MessageTypeName == typeof(PongMessage).Name)
            {
                //TODO add logs here.
            }
            else if (args.Message.MessageTypeName == typeof(PayloadMessageReceived).Name)
            {
                _persistence.DeleteMessageWithName(args.Message.MessageId);
            }
            else
            {
                var useCase = _useCaseFactory.GetUseCaseFor(args);
                useCase?.Execute();
            }
        }

        public void Send(MessageResponse messageResponse)
        {
            var defaultMessageResponse = _payloadResponseMapper.Map(messageResponse);
            _remoteApplicationManager.SendMessage(messageResponse.ReceiverName, defaultMessageResponse);
        }
    }
}