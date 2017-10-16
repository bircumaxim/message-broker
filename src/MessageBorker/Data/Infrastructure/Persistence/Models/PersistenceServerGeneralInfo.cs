using System;

namespace Persistence.Models
{
    public class PersistenceServerGeneralInfo
    {
        public DateTime ServerStartTime { get; set; }
        public int MessagesInQueue { get; set; }
    }
}