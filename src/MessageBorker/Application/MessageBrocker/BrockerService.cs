using System;
using System.Threading.Tasks;
using Data;
using Domain;

namespace MessageBrocker
{
    public class BrockerService : IRun
    {
        private readonly StartAndInitUseCase _startAndInitUseCase;
        private readonly Transport _transport;
        
        public BrockerService()
        {
            _transport = new Transport();
            _startAndInitUseCase = new StartAndInitUseCase(_transport);
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
            throw new System.NotImplementedException();
        }
    }
}