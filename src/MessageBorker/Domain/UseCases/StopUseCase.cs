using Domain.GateWays;

namespace Domain.UseCases
{
    public class StopUseCase
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
    }
}