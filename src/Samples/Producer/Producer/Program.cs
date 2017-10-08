using System;
using MessageBuss;

namespace Producer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var buss = BussFactory.Instance.GetBussFor("Brocker");
            buss.Topic(new UserOrderPayload(), "TestQueue2");
            Console.ReadKey();
        }
    }
}