using System;
using System.Collections.Generic;

namespace MessageBrocker
{
    internal class Program
    {
        private static readonly BrockerService BrockerService = new BrockerService();

        public static void Main(string[] args)
        {
            BrockerService.StartAsync();
            Console.WriteLine("Press any key to stop servr");
            Console.ReadKey();
        }
    }
}