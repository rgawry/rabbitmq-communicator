using Castle.Windsor;

namespace Chat
{
    class ServerProgram
    {
        private static IWindsorContainer _container = Bootstrapper.BootstrapContainer();
        private static IServerBus _severBus;
        private static ChitChatServer _chatServer;

        static void Main(string[] args)
        {
            try
            {
                _severBus = _container.Resolve<IServerBus>();
                _chatServer = _container.Resolve<ChitChatServer>();

                _severBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(_chatServer.SessionHandler);
                while (true) ;
            }
            finally
            {
                _container.Release(_chatServer);
                _container.Release(_severBus);
                _container.Dispose();
            }
        }
    }
}
