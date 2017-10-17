using System.IO;
using System.Threading.Tasks;
using Data;
using Data.Configuration.FileConfiguration;
using Domain.UseCases;
using MessageBroker.Infrastructure;

namespace MessageBroker
{
    public class BrokerService : IRun
    {
        private readonly StartAsyncUseCase _startAsyncUseCase;
        private readonly StartUseCase _startUseCase;
        private readonly StopUseCase _stopUseCase;
        private static readonly string ConfigFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Application/MessageBroker/config.xml");

        public BrokerService()
        {
            var configuration = new FileConfiguration(ConfigFilePath);
            var transport = new Transport(configuration);
            _startUseCase = new StartUseCase(transport);
            _startAsyncUseCase = new StartAsyncUseCase(transport);
            _stopUseCase = new StopUseCase(transport);
        }
        
        public void Start()
        {
            _startUseCase.Execute();
        }

        public Task StartAsync()
        {
            return _startAsyncUseCase.ExecuteAsync();
        }

        public void Stop()
        {
            _stopUseCase.Execute();
        }
    }
}