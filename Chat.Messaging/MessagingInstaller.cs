using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Chat
{
    public class MessagingInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<Configuration>(),
                Component.For<IMessageSerializer>()
                    .ImplementedBy<JsonMessageSerializer>(),
                Component.For<IRabbitMqServerBusFactory>()
                    .ImplementedBy<RabbitMqServerBusFactory>(),
                Component.For<IRabbitMqClientBusFactory>()
                    .ImplementedBy<RabbitMqClientBusFactory>(),
                Component.For<IServerBus>()
                    .UsingFactoryMethod(kernel => kernel.Resolve<IRabbitMqServerBusFactory>().Create(kernel.Resolve<IMessageSerializer>()))
                        .LifeStyle.Transient,
                Component.For<IClientBus>()
                    .UsingFactoryMethod(kernel => kernel.Resolve<IRabbitMqClientBusFactory>().Create(kernel.Resolve<IMessageSerializer>()))
                        .LifeStyle.Transient);
        }
    }
}
