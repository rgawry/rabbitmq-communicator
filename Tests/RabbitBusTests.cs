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
        IBus rabbitBus = new RabbitMqBus("session-exchange", "session-request");
        IServer server = new Server.Server();

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
            server.ListenForNewSession();

            var actualResponse = await rabbitBus.Request<OpenSessionRequest, OpenSessionResponse>(requestData);

            Assert.IsTrue(actualResponse.IsLogged);
        }
    }
}
