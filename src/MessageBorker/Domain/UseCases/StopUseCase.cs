﻿using System.Threading.Tasks;
using Domain.GateWays;

namespace Domain.UseCases
{
    public class StopUseCase : IUseCase
    {
        private readonly ITransportGateWay _transportGateWay;

        public StopUseCase(ITransportGateWay transportGateWay)
        {
            _transportGateWay = transportGateWay;
        }

        public void Execute()
        {
            _transportGateWay.Stop();
        }

        public Task ExecuteAsync()
        {
            //TODO do interface segregatio here.
            throw new System.NotImplementedException();
        }
    }
}