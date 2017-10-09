using System.IO;
using System.Threading.Tasks;
using Data;
using Data.Configuration;
using Data.Configuration.FileConfiguration;
using Domain.UseCases;
using MessageBrocker.Infrastructure;

namespace MessageBrocker
{
    public class BrockerService : IRun
    {
        private readonly StartAsyncUseCase _startAsyncUseCase;
        private readonly StartUseCase _startUseCase;
        private readonly StopUseCase _stopUseCase;
        private readonly Transport _transport;
        private readonly IConfiguration _configuration;
        private static readonly string ConfigFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Application/MessageBrocker/config.xml");

        public BrockerService()
        {
            _configuration = new FileConfiguration(ConfigFilePath);
            _transport = new Transport(_configuration);
            _startUseCase = new StartUseCase(_transport);
            _startAsyncUseCase = new StartAsyncUseCase(_transport);
            _stopUseCase = new StopUseCase(_transport);
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