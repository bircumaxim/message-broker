using System;
using MessageBuss.Buss;
using MessageBuss.Buss.Events;

namespace Consumer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var buss = BussFactory.Instance.GetBussFor("Broker");
            buss.Subscribe("TestQueue2");
            buss.Subscribe("TestQueue3");
            buss.MessageReceived += OnMessageReceived;
            Console.WriteLine("done");
            Console.ReadKey();
            buss.Dispose();
            Console.ReadKey();
        }

        public static void OnMessageReceived(object sender, MessegeReceviedEventArgs args)
        {
            var userOrderPayload = args.Payload as UserOrderPayload;
            Console.WriteLine(userOrderPayload?.ToString());
        }
    }
}