using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Chat
{
    public class ServerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IMessagingProviderFactory>()
                    .ImplementedBy<MessagingProviderFactory>(),
                Component.For<IMessagingProvider>()
                    .UsingFactoryMethod(kernel => kernel.Resolve<IMessagingProviderFactory>().Create()),
                Component.For<IServerBus>()
                    .ImplementedBy<ServerBus>(),
                Component.For<IDisplay>()
                    .ImplementedBy<ConsoleDisplay>(),
                Component.For<ChatServer>());
        }
    }
}
