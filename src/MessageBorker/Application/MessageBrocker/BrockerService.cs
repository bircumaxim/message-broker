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
        private readonly StartUseCase _startUseCase;
        private readonly StopUseCase _stopUseCase;
        private readonly MessageReceivedUseCase _messageReceivedUse;
        private readonly Transport _transport;
        
        public BrockerService()
        {
            _messageReceivedUse = new MessageReceivedUseCase();
            _transport = new Transport();
            _startUseCase = new StartUseCase(_transport);
            _stopUseCase = new StopUseCase(_transport);
        }

        public void Start()
        {
            _startUseCase.Execute();
        }

        public Task StartAsync()
        {
            return _startUseCase.ExecuteAsync();
        }

        public void Stop()
        {
            _stopUseCase.Execute();
        }
    }
}