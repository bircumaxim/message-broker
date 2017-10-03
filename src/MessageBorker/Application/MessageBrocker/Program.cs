using System;
using System.Net.Sockets;
using log4net;

namespace MessageBrocker
{
    internal class Program
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Program));
        private static readonly BrockerService BrockerService = new BrockerService();

        public static void Main(string[] args)
        {   
            _logger.Info("Starting message broker");
            BrockerService.StartAsync();
            _logger.Info("Press any key to stop");
            Console.ReadKey();
            BrockerService.Stop();
            Console.ReadKey();
        }
    }
}