using Castle.Windsor;
using System;
using System.Threading.Tasks;

namespace Chat
{
    class ClientProgram
    {
        private static IWindsorContainer _container = Bootstrapper.BootstrapContainer();
        private static IClientBus _clientBus;

        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    _clientBus = _container.Resolve<IClientBus>();

                    while (true)
                    {
                        Console.WriteLine("Login: ");
                        var login = Console.ReadLine();
                        var request = new OpenSessionRequest { UserName = login };
                        var response = await _clientBus.Request<OpenSessionRequest, OpenSessionResponse>(request);

                        if (response.IsLogged)
                        {
                            Console.WriteLine("Loged in as '" + login + "'");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Cant log in as '" + login + "'");
                        }
                    }
                }
                finally
                {
                    _container.Release(_clientBus);
                    _container.Dispose();
                }
            }).Wait();
        }
    }
}
