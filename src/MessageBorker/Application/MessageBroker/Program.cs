using System;
using log4net;

namespace MessageBroker
{
    internal class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        private static readonly BrokerService BrokerService = new BrokerService();

        public static void Main(string[] args)
        {   
            BrokerService.StartAsync();
            Console.ReadKey();
            BrokerService.Stop();
            Console.WriteLine("\nServer stoped.\nPress any key to exit.");
            Console.ReadKey(); 
        }
    }
}