using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Chat
{
    public class ClientInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IMessagingProviderFactory>()
                    .ImplementedBy<MessagingProviderFactory>(),
                Component.For<IMessagingProvider>()
                    .UsingFactoryMethod(kernel => kernel.Resolve<IMessagingProviderFactory>().Create()),
                Component.For<IClientBus>()
                    .ImplementedBy<ClientBus>()
                        .DependsOn(Dependency.OnValue("requestStream", container.Resolve<Configuration>().QueueRequestName))
                            .LifeStyle.Transient,
                Component.For<IDisplay>()
                    .ImplementedBy<ConsoleDisplay>(),
                Component.For<ChatClient>()
                    .LifeStyle.Transient);
        }
    }
}
