using System;
using log4net;

namespace MessageBrocker
{
    internal class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        private static readonly BrockerService BrockerService = new BrockerService();

        public static void Main(string[] args)
        {   
            BrockerService.StartAsync();
            Console.ReadKey();
            BrockerService.Stop();
            Console.WriteLine("\nServer stoped.\nPress any key to exit.");
            Console.ReadKey(); 
        }
    }
}