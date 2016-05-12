using Castle.Windsor;
using System.Threading.Tasks;

namespace Chat
{
    class ClientProgram
    {
        private static IWindsorContainer _container = Bootstrapper.BootstrapContainer();
        private static ChatClient _chatClient;

        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    _chatClient = _container.Resolve<ChatClient>();
                    await _chatClient.TryLogIn();
                }
                finally
                {
                    _container.Dispose();
                }
            }).Wait();
        }
    }
}
