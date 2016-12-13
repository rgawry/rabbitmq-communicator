using Castle.Windsor;

namespace Chat
{
    class ClientProgram
    {
        private static IWindsorContainer _container = Bootstrapper.BootstrapContainer();
        private static ChatClient _chatClient;

        static void Main(string[] args)
        {
            try
            {
                _chatClient = _container.Resolve<ChatClient>();
                while (true) ;
            }
            finally
            {
                _container.Dispose();
            }
        }
    }
}
