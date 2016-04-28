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
    [TestFixture, Timeout(30000)]
    public class RabbitBusTests
    {
        [Test]
        public async Task ShouldReceiveSentMessage()
        {
            IBus rabbitBus = new RabbitMqBus("session-exchange", "session-request");
            var requestData = new OpenSessionRequest { Login = "radek" };
            IServer server = new Server.Server();

            server.ListenForNewSession();

            var actualResponse = await rabbitBus.Request<OpenSessionRequest, OpenSessionResponse>(requestData);

            Assert.IsTrue(actualResponse.IsLogged);
        }
    }
}
