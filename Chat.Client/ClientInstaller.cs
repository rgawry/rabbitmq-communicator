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
                Component.For<IRabbitMqClientBusFactory>()
                    .ImplementedBy<RabbitMqClientBusFactory>(),
                Component.For<IClientBus>()
                    .UsingFactoryMethod(kernel => kernel.Resolve<IRabbitMqClientBusFactory>().Create())
                        .LifeStyle.Transient,
                Component.For<IDisplay>()
                    .ImplementedBy<ConsoleDisplay>(),
                Component.For<ChatClient>()
                    .LifeStyle.Transient);
        }
    }
}
