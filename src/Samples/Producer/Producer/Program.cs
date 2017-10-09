using System;
using MessageBuss.Buss;

namespace Producer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var buss = BussFactory.Instance.GetBussFor("Brocker");
            while (true)
            {
                buss.Fanout(new UserOrderPayload {UserName = "Joric", OrderId = 123});
                Console.ReadKey();
            }
        }
    }
}