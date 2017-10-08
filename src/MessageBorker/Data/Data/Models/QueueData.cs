using System.Collections.Concurrent;
using Domain.Infrastructure;

namespace Data.Models
{
    public class QueueData<T>
    {
        public string Name { get; set; }
        private readonly ConcurrentQueue<T> _concurrentQueue;

        public QueueData()
        {
            _concurrentQueue = new ConcurrentQueue<T>();
        }

        public void Enqueue(T message)
        {
            _concurrentQueue.Enqueue(message);
        }

        public T Dequeue()
        {
            T message;
            _concurrentQueue.TryDequeue(out message);
            return message;
        }
    }
}