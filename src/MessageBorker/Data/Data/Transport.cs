using System.Threading.Tasks;
using Data.Configuration;
using Data.Events;
using Data.Mappers;
using Domain.GateWays;
using Domain.Models;
using log4net;
using Messages.Payload;

namespace Data
{
    public class Transport : ITransportGateWay
    {
        private readonly ILog _logger;
        private readonly RemoteApplicationManager _remoteApplicationManager;
        private readonly CommandFactory _commandFactory;
        private readonly SubscribtionManager _subscribtionManager;
        private readonly Persistence _persistence;

        public Transport(IConfiguration configuration)
        {
            _persistence = new Persistence(configuration.GetPersistenceConfiguration());
            _persistence.QueueUpdate += OnQueueUpdate;
            _logger = LogManager.GetLogger(GetType());
            _remoteApplicationManager = new RemoteApplicationManager(configuration);
            _remoteApplicationManager.RemoteApplicationMessageReceived += RemoteApplicationMessageReceived;
            _subscribtionManager = new SubscribtionManager(_remoteApplicationManager, _persistence);
            _commandFactory = new CommandFactory(this, _persistence, _remoteApplicationManager, _subscribtionManager);
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
            _persistence.QueueUpdate -= OnQueueUpdate;
            _remoteApplicationManager.RemoteApplicationMessageReceived -= RemoteApplicationMessageReceived;
            _remoteApplicationManager.Stop();
        }
        
        private void OnQueueUpdate(object sender, QueueUpdateEventArgs args)
        {
            _subscribtionManager.NotifySubscribers(args.QueueName);
        }

        private void RemoteApplicationMessageReceived(object seneder, RemoteApplicationMessageReceivedEventArgs args)
        {
            var command = _commandFactory.GetCommandFor(args);
            command?.Execute();
            _logger.Debug($"Executed {command?.GetType().Name} for application with id=\"{args.Application.Name}\"");
        }

        public void Send(Message message)
        {
            var payloadMessage = MappersPull.Instance.Map<Message, PayloadMessage>(message);
            _remoteApplicationManager.SendMessage(message.ReceiverName, payloadMessage);
        }
    }
}