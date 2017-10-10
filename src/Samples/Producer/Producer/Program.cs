using System;
using MessageBuss.Buss;

namespace Producer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var buss = BussFactory.Instance.GetBussFor("Brocker");
            buss.Ping();
            buss.MessageReceived += (sender, eventArgs) => { Console.WriteLine("Server is alive"); };
            buss.Fanout(new UserOrderPayload());
            Console.ReadKey();
            buss.Dispose();
            Console.WriteLine("Producer was closed.\nPress any key to exit");
            Console.ReadLine();
        }
    }
}