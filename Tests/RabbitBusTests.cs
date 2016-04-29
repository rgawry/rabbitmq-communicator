using Common;
using NUnit.Framework;
using Server;
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
        RabbitMqClientBus rabbitBus = new RabbitMqClientBus("session-exchange", "session-request");
        RabbitMqServerBus server = new RabbitMqServerBus();

        [TearDown]
        public void CleanUp()
        {
            rabbitBus.Dispose();
            server.Dispose();
        }

        [Test]
        public async Task ShouldReceiveSentMessage()
        {
            var requestData = new OpenSessionRequest { Login = "radek" };
            server.AddHandler(o => new OpenSessionResponse { IsLogged = true });

            var actualResponse = await rabbitBus.Request<OpenSessionRequest, OpenSessionResponse>(requestData);

            Assert.IsTrue(actualResponse.IsLogged);
        }

        [Test]
        public async Task ShouldMatchRequestWithResponse()
        {
            var request1 = new OpenSessionRequest { Login = "login1" };
            var request2 = new OpenSessionRequest { Login = "login2" };
            var rabbitBus = new RabbitMqClientBus("session-exchange", "session-request");
            var server = new RabbitMqServerBus();

            server.AddHandler(o =>
            {
                if(o.Login == "login1") Thread.Sleep(500);
                return new OpenSessionResponse { IsLogged = o.Login == "login1" ? true : false };
            });

            var response1 = await rabbitBus.Request<OpenSessionRequest, OpenSessionResponse>(request1);
            var response2 = await rabbitBus.Request<OpenSessionRequest, OpenSessionResponse>(request2);

            Assert.IsTrue(response1.IsLogged);
            Assert.IsFalse(response2.IsLogged);
        }
    }
}
