namespace Data.Commands
{
    public class UnsubscribeCommand : ICommand
    {
        private readonly string _remoteApplicationName;
        private readonly SubscribtionManager _subscribtionManager;

        public UnsubscribeCommand(string remoteApplicationName, SubscribtionManager subscribtionManager)
        {
            _remoteApplicationName = remoteApplicationName;
            _subscribtionManager = subscribtionManager;
        }

        public void Execute()
        {
            _subscribtionManager.RemoveSubscription(_remoteApplicationName);
        }
    }
}