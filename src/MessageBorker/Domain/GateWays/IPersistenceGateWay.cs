using System.Collections.Generic;
using Domain.Exhcanges;

namespace Domain.GateWays
{
    public interface IPersistenceGateWay
    {
        List<Exchange> GetExchanges();
    }
}