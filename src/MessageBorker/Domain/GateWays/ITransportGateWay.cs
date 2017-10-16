using System.Threading.Tasks;
using Domain.Models;

namespace Domain.GateWays
{
    public interface ITransportGateWay
    {
        void Start();
        Task StartAsync();
        void Stop();
        void Send(Message message);
    }
}