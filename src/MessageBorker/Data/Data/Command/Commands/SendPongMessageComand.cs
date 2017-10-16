using Messages.Connection;

namespace Data.Commands
{
    public class SendPongMessageComand : ICommand
    {
        private readonly string _applicationName;
        private readonly RemoteApplicationManager _remoteApplicationManager;

        public SendPongMessageComand(string applicationName, RemoteApplicationManager remoteApplicationManager)
        {
            _applicationName = applicationName;
            _remoteApplicationManager = remoteApplicationManager;
        }
        
        public void Execute()
        {
            _remoteApplicationManager.SendMessage(_applicationName, new PongMessage());
        }
    }
}