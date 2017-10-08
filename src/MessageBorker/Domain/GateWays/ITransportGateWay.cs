using System;
using System.Threading.Tasks;
using Domain.Exhcanges;
using Domain.Messages;

namespace Domain.GateWays
{
    public interface ITransportGateWay
    {
        void Start();
        Task StartAsync();
        void Stop();
    }
}