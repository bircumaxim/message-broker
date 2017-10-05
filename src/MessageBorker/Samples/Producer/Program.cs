using MessageBuss;

namespace Producer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Buss.Instance.Send(new TestMessage());
            while (true)
            {
                
            }
        }
    }
}