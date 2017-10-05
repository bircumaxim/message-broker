namespace MessageBuss
{
    public class Buss
    {
        private static Buss _instance;
        public static Buss Instance => _instance ?? (_instance = new Buss());

        private Buss()
        {
            
        }

        public void Send<T>(T message)
        {
            
        }

        public void Fanout()
        {
            
        }
    }
}