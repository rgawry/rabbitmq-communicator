using Chat.Messaging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture, Timeout(1000)]
    public class RabbitBusTests
    {
        [Test]
        public async Task ShouldReceiveSentMessage()
        {
            using (var rabbitBus = new RabbitMqClientBus("session-exchange", "session-request"))
            using (var serverBus = new RabbitMqServerBus())
            {
                var requestData = new OpenSessionRequest { Login = "radek" };
                serverBus.AddHandler(o => new OpenSessionResponse { IsLogged = true });

                var actualResponse = await rabbitBus.Request<OpenSessionRequest, OpenSessionResponse>(requestData);

                Assert.IsTrue(actualResponse.IsLogged);
            }
        }

        [Test]
        public async Task ShouldMatchRequestWithResponse()
        {
            using (var rabbitBus = new RabbitMqClientBus("session-exchange", "session-request"))
            using (var serverBus = new RabbitMqServerBus())
            {

                var request1 = new OpenSessionRequest { Login = "login1" };
                var request2 = new OpenSessionRequest { Login = "login2" };

                serverBus.AddHandler(o =>
                {
                    if (o.Login == "login1") Thread.Sleep(500);
                    return new OpenSessionResponse { IsLogged = o.Login == "login1" ? true : false };
                });

                var response1 = await rabbitBus.Request<OpenSessionRequest, OpenSessionResponse>(request1);
                var response2 = await rabbitBus.Request<OpenSessionRequest, OpenSessionResponse>(request2);

                Assert.IsTrue(response1.IsLogged);
                Assert.IsFalse(response2.IsLogged);
            }
        }
    }
}
