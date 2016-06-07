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
                Component.For<IRoomService>()
                    .ImplementedBy<RoomService>(),
                Component.For<IRequestResponseService<OpenSessionRequest, OpenSessionResponse>>()
                    .ImplementedBy<SessionService>(),
                Component.For<IRequestService<JoinRoomRequest>>()
                    .ImplementedBy<JoinRoomService>(),
                Component.For<IRequestResponseService<TokenRequest, TokenResponse>>()
                    .ImplementedBy<TokenService>(),
                Component.For<IServerBus>()
                    .ImplementedBy<ServerBus>()
                        .DependsOn(Dependency.OnAppSettingsValue("requestName", "queue-request-name")),
                Component.For<ChatServer>());
        }
    }
}
