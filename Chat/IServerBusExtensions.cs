using Castle.Windsor;

namespace Chat
{
    public static class IServerBusExtensions
    {
        public static void RegisterHandlers(this IServerBus serverBus, IWindsorContainer container)
        {
            //var requestResponseServices = container.ResolveAll(typeof(IRequestResponseService<,>));
            //var requestServices = container.ResolveAll(typeof(IRequestService<>));

            //foreach (var service in requestResponseServices)
            //{
            //    var serviceType = service.GetType();
            //    var genericTypes = serviceType.GetGenericArguments();
            //    var serverBusType = serverBus.GetType();
            //    var method = serverBusType.GetMethod("AddHandler");
            //    var gm = method.MakeGenericMethod(genericTypes);
            //    gm.Invoke(serverBus, new object[] { serviceType.GetMethod("Handle") });
            //}

            //foreach (var service in requestServices)
            //{
            //    var serviceType = service.GetType();
            //    var genericTypes = serviceType.GetGenericArguments();
            //    var serverBusType = serverBus.GetType();
            //    var method = serverBusType.GetMethod("AddHandler");
            //    var gm = method.MakeGenericMethod(genericTypes);
            //    gm.Invoke(serverBus, new object[] { serviceType.GetMethod("Handle") });
            //}

            var sessionService = container.Resolve<IRequestResponseService<OpenSessionRequest, OpenSessionResponse>>();
            var joinRoomService = container.Resolve<IRequestService<JoinRoomRequest>>();
            var tokenService = container.Resolve<IRequestResponseService<TokenRequest, TokenResponse>>();

            serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(sessionService.Handle);
            serverBus.AddHandler<TokenRequest, TokenResponse>(tokenService.Handle);
            serverBus.AddHandler<JoinRoomRequest>(joinRoomService.Handle);
        }
    }
}
