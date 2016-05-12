using Castle.Windsor;

namespace Chat
{
    class ServerProgram
    {
        private static IWindsorContainer _container = Bootstrapper.BootstrapContainer();
        private static ChatServer _chatServer;

        static void Main(string[] args)
        {
            try
            {
                _chatServer = _container.Resolve<ChatServer>();
                while (true) ;
            }
            finally
            {
                _container.Dispose();
            }
        }
    }
}
