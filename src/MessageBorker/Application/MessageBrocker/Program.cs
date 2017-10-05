using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;

namespace MessageBrocker
{
    internal class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        private static readonly BrockerService BrockerService = new BrockerService();

        public static void Main(string[] args)
        {   
            Logger.Info("Starting message broker");
            BrockerService.StartAsync();
            Logger.Info("Press any key to stop the server");
            Console.ReadKey();
            BrockerService.Stop();
            Console.ReadKey(); 
            Logger.Info("Press any key to exit");
        }
    }
}