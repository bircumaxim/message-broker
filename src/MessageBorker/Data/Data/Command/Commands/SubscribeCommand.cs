using Messages.Subscribe;

namespace Data.Commands
{
    public class SubscribeCommand : ICommand
    {
        private readonly Persistence _persistence;
        private readonly SubscribeMessage _subscribeMessage;
        private readonly string _remoteApplicationName;
        private readonly SubscribtionManager _subscribtionManager;

        public SubscribeCommand(string remoteApplicationName, SubscribeMessage subscribeMessage,
            Persistence persistence, SubscribtionManager subscribtionManager)
        {
            _remoteApplicationName = remoteApplicationName;
            _subscribeMessage = subscribeMessage;
            _persistence = persistence;
            _subscribtionManager = subscribtionManager;
        }

        public void Execute()
        {
            _persistence.PersistSubscription(_subscribeMessage, _remoteApplicationName);
            _subscribtionManager.AddSubscrption(_subscribeMessage.QueueName, _remoteApplicationName);
            _subscribtionManager.NotifySubscribers(_subscribeMessage.QueueName);
        }
    }
}