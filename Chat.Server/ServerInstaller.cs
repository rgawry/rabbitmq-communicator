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
                Component.For<IServerBus>()
                    .ImplementedBy<ServerBus>()
                        .DependsOn(Dependency.OnAppSettingsValue("requestName", "queue-request-name")),
                Component.For<IDisplay>()
                    .ImplementedBy<ConsoleDisplay>(),
                Component.For<ChatServer>());
        }
    }
}
