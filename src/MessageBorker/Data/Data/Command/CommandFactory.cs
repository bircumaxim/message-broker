using Data.Commands;
using Data.Events;
using Messages;
using Messages.Connection;
using Messages.Payload;
using Messages.ServerInfo;
using Messages.Subscribe;

namespace Data
{
    public class CommandFactory
    {
        private readonly Persistence _persistence;
        private readonly RemoteApplicationManager _remoteApplicationManager;
        private readonly UseCaseFactory _useCaseFactory;
        private readonly Transport _transport;
        private readonly SubscribtionManager _subscribtionManager;

        public CommandFactory(Transport transport, Persistence persistence,
            RemoteApplicationManager remoteApplicationManager, SubscribtionManager subscribtionManager)
        {
            _transport = transport;
            _persistence = persistence;
            _remoteApplicationManager = remoteApplicationManager;
            _subscribtionManager = subscribtionManager;
            _useCaseFactory = new UseCaseFactory(_persistence, transport);
        }

        public ICommand GetCommandFor(RemoteApplicationMessageReceivedEventArgs args)
        {
            if (args.Message.MessageTypeName == typeof(CloseConnectionRequest).Name)
            {
                return new StopRemoteApplicationCommand(args.Application.Name, _remoteApplicationManager);
            }
            if (args.Message.MessageTypeName == typeof(PingMessage).Name)
            {
                return new SendPongMessageComand(args.Application.Name, _remoteApplicationManager);
            }
            if (args.Message.MessageTypeName == typeof(ServerGerneralInfoRequest).Name)
            {
                return new SendServerGeneralInfoCommand(args.Application.Name, _remoteApplicationManager, _persistence);
            }
            if (args.Message.MessageTypeName == typeof(PongMessage).Name)
            {
                return new SendPingMessageCommand(args.Application.Name);
            }
            if (args.Message.MessageTypeName == typeof(PayloadMessageReceived).Name)
            {
                return new DeleteMessageCommand(args.Message.MessageId, _persistence);
            }
            if (args.Message.MessageTypeName == typeof(SubscribeMessage).Name)
            {
                var message = args.Message as SubscribeMessage;
                return new SubscribeCommand(args.Application.Name, message, _persistence, _subscribtionManager);
            }
            var useCase = _useCaseFactory.GetUseCaseFor(args);
            return new ExecuteDomainUseCaseCommand(useCase);
        }
    }
}