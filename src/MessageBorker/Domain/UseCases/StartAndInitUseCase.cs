using System.Threading.Tasks;
using Domain.GateWays;

namespace Domain
{
    public class StartAndInitUseCase : IUseCase
    {
        private readonly ITransportGateWay _transportGateWay;

        public StartAndInitUseCase(ITransportGateWay transportGateWay)
        {
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