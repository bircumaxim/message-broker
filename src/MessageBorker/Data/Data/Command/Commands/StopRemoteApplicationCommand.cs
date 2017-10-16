namespace Data.Commands
{
    public class StopRemoteApplicationCommand : ICommand
    {
        private readonly string _applicationName;
        private readonly RemoteApplicationManager _remoteApplicationManager;

        public StopRemoteApplicationCommand(string applicationName, RemoteApplicationManager remoteApplicationManager)
        {
            _applicationName = applicationName;
            _remoteApplicationManager = remoteApplicationManager;
        }

        public void Execute()
        {
            _remoteApplicationManager.StopRemoteApplication(_applicationName);
        }
    }
}