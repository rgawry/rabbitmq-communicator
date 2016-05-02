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
    [TestFixture]
    //[Timeout(2000)]
    public class ClientBusServerBusIntegrationTests
    {
        [Test]
        public async Task ShouldReceiveSentMessage()
        {
            var exchangeName = "session-exchange";
            var queueName = "session-request";

            using (var clientBus = new RabbitMqClientBus(exchangeName, queueName))
            using (var serverBus = new RabbitMqServerBus(exchangeName, queueName))
            {
                clientBus.Init();
                serverBus.Init();

                var requestData = new OpenSessionRequest { UserName = "login1" };
                serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(o => new OpenSessionResponse { IsLogged = true });

                var actualResponse = await clientBus.Request<OpenSessionRequest, OpenSessionResponse>(requestData);

                Assert.IsTrue(actualResponse.IsLogged);
            }
        }

        [Test]
        //[Ignore("how to fail this test")]
        public async Task ShouldMatchRequestWithResponse()
        {
            var exchangeName = "session-exchange";
            var queueName = "session-request";

            using (var clientBus_channel1 = new RabbitMqClientBus(exchangeName, queueName))
            using (var clientBus_channel2 = new RabbitMqClientBus(exchangeName, queueName))
            using (var serverBus = new RabbitMqServerBus(exchangeName, queueName))
            {
                clientBus_channel1.Init();
                clientBus_channel2.Init();
                serverBus.Init();

                var request1 = new OpenSessionRequest { UserName = "login1" };
                var request2 = new OpenSessionRequest { UserName = "login2" };

                serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(o =>
                {
                    if (o.UserName == "login1") Thread.Sleep(500);
                    return new OpenSessionResponse { IsLogged = o.UserName == "login1" ? true : false };
                });

                var response1 = await clientBus_channel1.Request<OpenSessionRequest, OpenSessionResponse>(request1);
                var response2 = await clientBus_channel2.Request<OpenSessionRequest, OpenSessionResponse>(request2);

                Assert.IsTrue(response1.IsLogged);
                Assert.IsFalse(response2.IsLogged);
            }
        }
    }
}
