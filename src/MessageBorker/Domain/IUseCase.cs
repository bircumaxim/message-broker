using System.Threading.Tasks;

namespace Domain
{
    public interface IUseCase
    {
        void Execute(); 
        Task ExecuteAsync();
    }
    
    public interface IUseCase<T>
    {
        Task<T> ExecuteAsync();
    }
}