using System.Threading.Tasks;

namespace Transport
{
    
    public interface IRun
    {
        void Start();
        Task StartAsync();
        void Stop();
    }
    
    public interface IRun<T>
    {
        void Start();
        Task<T> StartAsync();
        void Stop();
    }
}