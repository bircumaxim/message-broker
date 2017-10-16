using System.Collections.Concurrent;
using Domain.Infrastructure;

namespace Domain
{
    public class Queue<T>
    {
        public string Name { get; set; }
        private readonly ConcurrentQueue<T> _concurrentQueue;

        public Queue()
        {   
            _concurrentQueue = new ConcurrentQueue<T>();
        }

        public void Enqueue(T message)
        {
            _concurrentQueue.Enqueue(message);
        }

        public bool IsEmpty()
        {
            return _concurrentQueue.IsEmpty;
        }
        
        public T Dequeue()
        {
            T message;
            _concurrentQueue.TryDequeue(out message);
            return message;
        }
    }
}