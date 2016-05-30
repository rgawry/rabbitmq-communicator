﻿using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Chat
{
    public class ServerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ISessionHandler>()
                    .ImplementedBy<SessionHandler>(),
                Component.For<IServerBus>()
                    .ImplementedBy<ServerBus>()
                        .DependsOn(Dependency.OnAppSettingsValue("requestName", "queue-request-name")),
                Component.For<ChatServer>());
        }
    }
}
