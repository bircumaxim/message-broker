using System;
using System.Threading.Tasks;
using Data.Events;
using log4net;
using Transport;
using Transport.Connectors;
using Transport.Events;

namespace Data
{
    public class RemoteApplication : IRun
    {
        public string Name { get; }
        private readonly ILog _logger;
        private readonly IConnector _connector;
        public event MessageReceivedFromRemoteApplicationHandler MessageReceivedFromRemoteApplication;
        
        public RemoteApplication(IConnector connector)
        {
            Name = Guid.NewGuid().ToString();
            _connector = connector;
            _connector.MessageReceived += OnMessageReceived;
            _logger = LogManager.GetLogger(GetType());
            Validate();
            _logger.Info($"New remote application with id {Name}");
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
//            MessageUseCaseFactory.GetUseCaseForMessage(args.Message);
        }

        public void Start()
        {
            _connector.Start();
        }

        public Task StartAsync()
        {
            _logger.Info("Starting remote applicatoin");
            return _connector.StartAsync();
        }

        public void Stop()
        {
            _connector.Stop();
        }
        
        private void Validate()
        {
            if (_connector != null) return;
            throw new NullReferenceException();
        }
    }
}