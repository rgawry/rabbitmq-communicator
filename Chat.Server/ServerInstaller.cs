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
                Component.For<IRabbitMqServerBusFactory>()
                    .ImplementedBy<RabbitMqServerBusFactory>(),
                Component.For<IServerBus>()
                    .UsingFactoryMethod(kernel => kernel.Resolve<IRabbitMqServerBusFactory>().Create()),
                Component.For<IDisplay>()
                    .ImplementedBy<ConsoleDisplay>(),
                Component.For<ChatServer>());
        }
    }
}
