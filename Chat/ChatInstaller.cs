using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Chat
{
    public class ChatInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IMessagingProviderFactory>()
                    .ImplementedBy<MessagingProviderFactory>(),
                Component.For<IMessagingProvider>()
                    .UsingFactoryMethod(kernel => kernel.Resolve<IMessagingProviderFactory>().Create())
                        .LifeStyle.Transient,
                Component.For<Configuration>(),
                Component.For<IMessageSerializer>()
                    .ImplementedBy<JsonMessageSerializer>());
        }
    }
}
