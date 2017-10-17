namespace Domain.Models
{
    public static class MessageCloneFactory
    {
        public static IMessage GetClone(IMessage message)
        {
            return message.MakeCopy();
        }
    }
}