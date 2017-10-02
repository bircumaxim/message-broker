using System.Threading.Tasks;

namespace Domain.GateWays
{
    public interface ITransportGateWay
    {
        void Start();
        Task StartAsync();
        void Stop();
    }
}