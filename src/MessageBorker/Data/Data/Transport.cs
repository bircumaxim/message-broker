using System;
using System.IO;
using System.Threading.Tasks;
using Data.Configuration;
using Data.Events;
using Data.Mappers;
using Domain.GateWays;
using Domain.Messages;
using Messages;
using Messages.Connection;
using Messages.ServerInfo;
using Serialization;
using Serialization.Deserializers;
using Serialization.WireProtocols;

namespace Data
{
    public class Transport : ITransportGateWay
    {
        private readonly PayloadResponseMapper _payloadResponseMapper;
        private readonly RemoteApplicationManager _remoteApplicationManager;
        private readonly UseCaseFactory _useCaseFactory;

        public Transport(IConfiguration configuration)
        {
            var persistence = new Persistence(configuration);
            _useCaseFactory = new UseCaseFactory(persistence, this);
            _remoteApplicationManager = new RemoteApplicationManager(configuration);
            _remoteApplicationManager.RemoteApplicationMessageReceived += RemoteApplicationMessageReceived;
            _payloadResponseMapper = new PayloadResponseMapper();
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
            if (args.Message.MessageTypeName == typeof(CloseConnectionRequest).Name)
            {
                _remoteApplicationManager.StopRemoteApplication(args.Application.Name);
            }
            else if (args.Message.MessageTypeName == typeof(PingMessage).Name)
            {
                _remoteApplicationManager.SendMessage(args.Application.Name, new PongMessage());
            }
            else if (args.Message.MessageTypeName == typeof(PongMessage).Name)
            {
                //TODO add logs here.
            }
            else
            {
                var useCase = _useCaseFactory.GetUseCaseFor(args);
                useCase?.Execute();
            }
        }

        public void Send(MessageResponse messageResponse)
        {
            _remoteApplicationManager.SendMessage(messageResponse.ReceiverName,
                _payloadResponseMapper.Map(messageResponse));
        }
    }
}