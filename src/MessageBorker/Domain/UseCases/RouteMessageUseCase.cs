using System;
using Domain.Exhcanges;
using Domain.GateWays;
using Domain.Models;

namespace Domain.UseCases
{
    public class RouteMessageUseCase : IUseCase
    {
        private readonly IPersistenceGateWay _persistence;
        private readonly RouteMessage _routeMessage;
        private readonly Exchange _exchange;

        public RouteMessageUseCase(RouteMessage routeMessage, IPersistenceGateWay persistence)
        {
            _routeMessage = routeMessage;
            _persistence = persistence;
            _exchange = _persistence.GetExchangeFor(_routeMessage);
            Validate();
        }

        public void Execute()
        {
            _exchange.Route(_routeMessage);
            _persistence.PersistQueues(_exchange.Queues);
        }

        private void Validate()
        {
            if (_exchange == null || _routeMessage == null || _persistence == null)
            {
                throw new NullReferenceException();
            }
        }
    }
}