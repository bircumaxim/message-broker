using System.Threading.Tasks;
using Domain.GateWays;
using log4net;

namespace Domain.UseCases
{
    public class StartUseCase : IUseCase
    {
        private readonly ITransportGateWay _transportGateWay;
        
        public StartUseCase(ITransportGateWay transportGateWay)
        {
            _transportGateWay = transportGateWay;
        }

        public virtual void Execute()
        {
            _transportGateWay.Start();
        }
    }
}