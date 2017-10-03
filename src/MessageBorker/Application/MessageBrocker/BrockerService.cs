using System;
using System.Threading.Tasks;
using Data;
using Domain;
using Domain.UseCases;
using MessageBrocker.Infrastructure;

namespace MessageBrocker
{
    public class BrockerService : IRun
    {
        private readonly StartAndInitUseCase _startAndInitUseCase;
        private readonly StopUseCase _stopUseCase;
        private readonly Transport _transport;
        
        public BrockerService()
        {
            _transport = new Transport();
            _startAndInitUseCase = new StartAndInitUseCase(_transport);
            _stopUseCase = new StopUseCase(_transport);
        }

        public void Start()
        {
            _startAndInitUseCase.Execute();
        }

        public Task StartAsync()
        {
            return _startAndInitUseCase.ExecuteAsync();
        }

        public void Stop()
        {
            _stopUseCase.Execute();
        }
    }
}