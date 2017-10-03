using System;
using log4net;
using Transport.Connectors;

namespace Data
{
    public class RemoteApplication
    {
        private readonly ILog _logger;
        private readonly IConnector _connector;

        public RemoteApplication(IConnector connector)
        {
            _connector = connector;
            _logger = LogManager.GetLogger(GetType());
            Validate();
        }

        public void Validate()
        {
            if (_connector != null) return;
            _logger.Error("Socket could not be null");
            throw new NullReferenceException();
        } 
    }
}