using Castle.Windsor;

namespace Chat
{
    class ServerProgram
    {
        private static IWindsorContainer _container = Bootstrapper.BootstrapContainer();
        private static IServerBus _severBus;

        static void Main(string[] args)
        {
            try
            {
                _severBus = _container.Resolve<IServerBus>();
                var chatServer = new ChitChatServer();
                chatServer.Initialize();

                _severBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(chatServer.SessionHandler);
                while (true) ;
            }
            finally
            {
                _container.Release(_severBus);
                _container.Dispose();
            }
        }
    }
}
