using System;
using MessageBuss;

namespace Producer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                Buss.Instance.Send(new UserOrderPayload());
                Console.ReadKey();
            }
        }
    }
}