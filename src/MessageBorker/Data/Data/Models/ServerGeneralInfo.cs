using System;

namespace Data.Models
{
    public class ServerGeneralInfo
    {
        public DateTime ServerStartTime { get; set; }
        public int ConnectionsCount { get; set; }
        public int MessagesInQueue { get; set; }
    }
}