using System.IO;
using System.Threading.Tasks;
using Data.Configuration;
using Data.Events;
using Data.Mappers;
using Domain.GateWays;
using Domain.Messages;
using Messages;
using Serialization;
using Serialization.Deserializers;
using Serialization.WireProtocols;

namespace Data
{
    public class Transport : ITransportGateWay
    {
        private readonly DefaultMessageResponseMapper _defaultMessageResponseMapper;
        private readonly RemoteApplicationManager _remoteApplicationManager;
        private readonly UseCaseFactory _useCaseFactory;

        public Transport(IConfiguration configuration)
        {
            var persistence = new Persistence(configuration);
            _useCaseFactory = new UseCaseFactory(persistence, this);
            _remoteApplicationManager = new RemoteApplicationManager(configuration);
            _remoteApplicationManager.RemoteApplicationMessageReceived += RemoteApplicationMessageReceived;
            _defaultMessageResponseMapper = new DefaultMessageResponseMapper();
        }

        public void Start()
        {
            _remoteApplicationManager.Start();
        }

        public Task StartAsync()
        {
            return _remoteApplicationManager.StartAsync();
        }

        public void Stop()
        {
            _remoteApplicationManager.RemoteApplicationMessageReceived -= RemoteApplicationMessageReceived;
            _remoteApplicationManager.Stop();
        }

        private void RemoteApplicationMessageReceived(object seneder, RemoteApplicationMessageReceivedEventArgs args)
        {
            switch (args.Message.MessageTypeName)
            {
                case "CloseConnectionRequest":
                    _remoteApplicationManager.StopRemoteApplication(args.Application.Name);
                    break;
                case "PingMessage":
                    _remoteApplicationManager.SendMessage(args.Application.Name, new PongMessage());
                    break;
                case "PongMessage":
                    //TODO add logs here.
                    break;
                default:
                    var useCase = _useCaseFactory.GetUseCaseFor(args);
                    useCase?.Execute();
                    break;
            }
        }

        public void Send(MessageResponse messageResponse)
        {
            _remoteApplicationManager.SendMessage(messageResponse.ReceiverName,
                _defaultMessageResponseMapper.Map(messageResponse));
        }
    }
}