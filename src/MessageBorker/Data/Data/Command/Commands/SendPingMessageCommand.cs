using log4net;

namespace Data.Commands
{
    public class SendPingMessageCommand : ICommand
    {
        private readonly ILog _logger;
        private readonly string _applicationName;

        public SendPingMessageCommand(string applicationName)
        {
            _applicationName = applicationName;
            _logger = LogManager.GetLogger(GetType());
        }

        public void Execute()
        {
            _logger.Info($"Received pong message from client with id=\"{_applicationName}\"");
        }
    }
}