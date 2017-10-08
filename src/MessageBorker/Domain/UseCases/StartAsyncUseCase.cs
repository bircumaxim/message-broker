using System.Threading.Tasks;
using Domain.GateWays;

namespace Domain.UseCases
{
    public class StartAsyncUseCase
    {
        private readonly ITransportGateWay _transportGateWay;
        
        public StartAsyncUseCase(ITransportGateWay transportGateWay)
        {
            _transportGateWay = transportGateWay;
        }

        public Task ExecuteAsync()
        {
            return _transportGateWay.StartAsync();
        }
    }
}