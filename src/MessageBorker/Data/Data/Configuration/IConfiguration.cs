using System.Collections.Generic;
using System.Runtime.InteropServices;
using Data.Models;
using Persistence.Configuration;
using Serialization.WireProtocol;
using Transport;

namespace Data.Configuration
{
    public interface IConfiguration
    {
        List<IConnectionManager> GetConnectionManagers();

        IPersistenceConfiguration GetPersistenceConfiguration();
    }
}