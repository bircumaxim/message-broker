namespace Domain.Models
{
    public interface IMessage
    {
        IMessage MakeCopy();
    }
}