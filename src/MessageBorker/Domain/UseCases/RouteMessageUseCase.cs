using System;
using System.Xml.Schema;
using Domain.Exhcanges;
using Domain.GateWays;
using Domain.Messages;

namespace Domain.UseCases
{
    public class RouteMessageUseCase : IUseCase
    {
        private readonly IPersistenceGateWay _persistence;
        private readonly Message _message;
        private readonly Exchange _exchange;

        public RouteMessageUseCase(Message message, IPersistenceGateWay persistence)
        {
            _message = message;
            _persistence = persistence;
            _exchange = _persistence.GetExchangeFor(_message);
            Validate();
        }

        public void Execute()
        {
            _exchange.Route(_message);
            _persistence.PersistQueues(_exchange.Queues);
        }

        private void Validate()
        {
            if (_exchange == null || _message == null || _persistence == null)
            {
                throw new NullReferenceException();
            }
        }
    }
}