using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Chat
{
    [TestFixture]
    [Timeout(1000)]
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

                Assert.That(actualResponse.IsLogged, Is.True);
            }
        }

        [Test]
        public async Task ShouldMatchRequestWithResponse()
        {
            var exchangeName = "session-exchange";
            var queueName = "session-request";

            using (var clientBus = new RabbitMqClientBus(exchangeName, queueName))
            using (var serverBus = new RabbitMqServerBus(exchangeName, queueName))
            {
                clientBus.Init();
                serverBus.Init();

                var request1 = new OpenSessionRequest { UserName = "login1" };
                var request2 = new OpenSessionRequest { UserName = "login2" };

                serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(o =>
                {
                    if (o.UserName == "login1") Thread.Sleep(500);
                    return new OpenSessionResponse { IsLogged = o.UserName == "login1" ? true : false };
                });

                var response1AsTask = clientBus.Request<OpenSessionRequest, OpenSessionResponse>(request1);
                var response2AsTask = clientBus.Request<OpenSessionRequest, OpenSessionResponse>(request2);

                await Task.WhenAll(response1AsTask, response2AsTask);

                Assert.That(response1AsTask.Result.IsLogged, Is.True);
                Assert.That(response2AsTask.Result.IsLogged, Is.False);
            }
        }
    }
}
