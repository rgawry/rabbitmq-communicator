using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chat
{
    public static class IServerBusExtensions
    {
        public static void RegisterHandlers(this IServerBus serverBus, IWindsorContainer container)
        {
            var requestGenericArgsCount = 1;
            var requestResponseGenericArgsCount = 2;
            var services = container.Kernel.GetAssignableHandlers(typeof(IServiceTag)).Select(h => h.ComponentModel.Implementation);
            var requestServices = SelectServices(services, requestGenericArgsCount);
            var requestResponseServices = SelectServices(services, requestResponseGenericArgsCount);

            foreach (var service in requestServices)
            {
                RegisterHandler(serverBus, container, service, requestGenericArgsCount);
            }

            foreach (var service in requestResponseServices)
            {
                RegisterHandler(serverBus, container, service, requestResponseGenericArgsCount);
            }

            //var sessionService = container.Resolve<IRequestResponseService<OpenSessionRequest, OpenSessionResponse>>();
            //var joinRoomService = container.Resolve<IRequestService<JoinRoomRequest>>();
            //var tokenService = container.Resolve<IRequestResponseService<TokenRequest, TokenResponse>>();

            //serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(sessionService.Handle);
            //serverBus.AddHandler<TokenRequest, TokenResponse>(tokenService.Handle);
            //serverBus.AddHandler<JoinRoomRequest>(joinRoomService.Handle);
        }

        internal static IEnumerable<Type> SelectServices(IEnumerable<Type> services, int requestGenericArgsCount)
        {
            return services.Where(t => t.GetInterfaces().FirstOrDefault().GetGenericArguments().Length == requestGenericArgsCount);
        }

        internal static void RegisterHandler(IServerBus serverBus, IWindsorContainer container, Type service, int genericArgsCount)
        {
            var instance = container.Resolve(service.GetInterfaces().FirstOrDefault());
            var genericTypes = service.GetInterfaces().FirstOrDefault().GetGenericArguments();
            var serverBusType = serverBus.GetType();
            var method = serverBusType.GetMethods().Where(mi => mi.Name == "AddHandler" && mi.GetGenericArguments().Length == genericArgsCount).FirstOrDefault();
            var gm = method.MakeGenericMethod(genericTypes);
            var deleg = Delegate.CreateDelegate(gm.GetParameters().FirstOrDefault().ParameterType, instance, "Handle");
            gm.Invoke(serverBus, new object[] { deleg });
        }
    }
}
