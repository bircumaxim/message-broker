namespace Data.Commands
{
    public class DeleteMessageCommand : ICommand
    {
        private readonly string _messageId;
        private readonly Persistence _persistence;

        public DeleteMessageCommand(string messageId, Persistence persistence)
        {
            _messageId = messageId;
            _persistence = persistence;
        }

        public void Execute()
        {
            _persistence.DeleteMessageWithId(_messageId);
        }
    }
}