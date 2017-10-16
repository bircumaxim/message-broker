using System.Collections.Concurrent;

namespace Persistence.Models
{
    public class PersistenceQueue<T>
    {
        public string Name { get; set; }
        private readonly ConcurrentQueue<T> _concurrentQueue;
       
        public PersistenceQueue()
        {
            _concurrentQueue = new ConcurrentQueue<T>();
        }

        public void Enqueue(T message)
        {
            _concurrentQueue.Enqueue(message);
        }

        public int Count()
        {
            return _concurrentQueue.Count;
        }
        
        public T Dequeue()
        {
            T message;
            _concurrentQueue.TryDequeue(out message);
            return message;
        }
        
    }
}