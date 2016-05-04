using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Chat
{
    [TestFixture]
    [Timeout(2000)]
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
                await serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(o => new OpenSessionResponse { IsLogged = true });

                var actualResponse = await clientBus.Request<OpenSessionRequest, OpenSessionResponse>(requestData);

                Assert.That(actualResponse.IsLogged, Is.True);
            }
        }

        //[Test]
        //public async Task ShouldReceiveSentMessage2()
        //{
        //    var exchangeName = "session-exchange";
        //    var queueName = "session-request";

        //    using (var clientBus = new RabbitMqClientBus(exchangeName, queueName))
        //    using (var serverBus = new RabbitMqServerBus(exchangeName, queueName))
        //    {
        //        clientBus.Init();
        //        serverBus.Init();

        //        var requestData = new JoinRoomRequest { RoomName = "", Token = "login1" };
        //        await serverBus.AddHandler<JoinRoomRequest>(o => { });

        //        await clientBus.Request(requestData);


        //    }
        //}

        [Test]
        [Ignore("how to fail this test")]
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

                await serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(o =>
                {
                    if (o.UserName == "login1") Thread.Sleep(500);
                    return new OpenSessionResponse { IsLogged = o.UserName == "login1" ? true : false };
                });

                var response1 = await clientBus_channel1.Request<OpenSessionRequest, OpenSessionResponse>(request1);
                var response2 = await clientBus_channel2.Request<OpenSessionRequest, OpenSessionResponse>(request2);

                Assert.That(response1.IsLogged, Is.True);
                Assert.That(response2.IsLogged, Is.False);
            }
        }
    }
}
