using Castle.Windsor;

namespace Chat
{
    class ServerProgram
    {
        private static IWindsorContainer _container = Bootstrapper.BootstrapContainer();

        static void Main(string[] args)
        {
            try
            {
                _container.Resolve<IServerBus>();
                while (true) ;
            }
            finally
            {
                _container.Dispose();
            }
        }
    }
}
