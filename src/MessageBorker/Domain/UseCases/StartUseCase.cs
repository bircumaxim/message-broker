﻿using System.Threading.Tasks;
using Domain.GateWays;
using log4net;

namespace Domain.UseCases
{
    public class StartUseCase
    {
        private readonly ITransportGateWay _transportGateWay;
        private readonly ILog _logger;
        
        public StartUseCase(ITransportGateWay transportGateWay)
        {
            _logger = LogManager.GetLogger(GetType());
            _transportGateWay = transportGateWay;
        }

        public void Execute()
        {
            _transportGateWay.Start();
        }

        public Task ExecuteAsync()
        {
            return _transportGateWay.StartAsync();
        }
    }
}