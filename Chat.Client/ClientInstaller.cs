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
                Component.For<IClientBus>()
                    .ImplementedBy<ClientBus>()
                        .DependsOn(Dependency.OnAppSettingsValue("requestName", "queue-request-name"))
                            .LifeStyle.Transient,
                Component.For<ChatClient>()
                    .LifeStyle.Transient);
        }
    }
}
