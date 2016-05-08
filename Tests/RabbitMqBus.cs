using NUnit.Framework;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chat
{
    [TestFixture]
    [Timeout(1000)]
    public class RabbitMqBus
    {
        [Test]
        public async Task ShouldReceiveSentMessage()
        {
            var config = new Configuration();
            var messageSerializer = new JsonMessageSerializer();
            var exchangeName = "session-exchange";
            var queueName = "session-request";
            var connectionFactory = new ConnectionFactory { HostName = config.HostName, Port = config.Port };
            var connectionClient = connectionFactory.CreateConnection();
            var connectionServer = connectionFactory.CreateConnection();

            using (var clientBus = new RabbitMqClientBus(exchangeName, queueName, connectionClient, messageSerializer))
            using (var serverBus = new RabbitMqServerBus(exchangeName, queueName, connectionServer, messageSerializer))
            {
                clientBus.Init();
                serverBus.Init();

                var request = new OpenSessionRequest { UserName = "login1" };

                serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(req => new OpenSessionResponse { IsLogged = true });

                var response = await clientBus.Request<OpenSessionRequest, OpenSessionResponse>(request);

                Assert.That(response.IsLogged, Is.True);
            }
        }

        [Test]
        public async Task ShouldMatchRequestWithResponse()
        {
            var config = new Configuration();
            var messageSerializer = new JsonMessageSerializer();
            var exchangeName = "session-exchange";
            var queueName = "session-request";
            var connectionFactory = new ConnectionFactory { HostName = config.HostName, Port = config.Port };
            var connectionClient = connectionFactory.CreateConnection();
            var connectionServer = connectionFactory.CreateConnection();

            using (var clientBus = new RabbitMqClientBus(exchangeName, queueName, connectionClient, messageSerializer))
            using (var serverBus = new RabbitMqServerBus(exchangeName, queueName, connectionServer, messageSerializer))
            {
                clientBus.Init();
                serverBus.Init();

                var request1 = new OpenSessionRequest { UserName = "login1" };
                var request2 = new OpenSessionRequest { UserName = "login2" };

                serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(req =>
                {
                    if (req.UserName == "login1") Thread.Sleep(500);
                    return new OpenSessionResponse { IsLogged = req.UserName == "login1" ? true : false };
                });

                var response1AsTask = clientBus.Request<OpenSessionRequest, OpenSessionResponse>(request1);
                var response2AsTask = clientBus.Request<OpenSessionRequest, OpenSessionResponse>(request2);

                await Task.WhenAll(response1AsTask, response2AsTask);

                Assert.That(response1AsTask.Result.IsLogged, Is.True);
                Assert.That(response2AsTask.Result.IsLogged, Is.False);
            }
        }

        [Test]
        [Timeout(3000)]
        public void ShouldTimeout()
        {
            AsyncTestDelegate testDelegate = async () =>
            {
                var config = new Configuration();
                var messageSerializer = new JsonMessageSerializer();
                var exchangeName = "session-exchange";
                var queueName = "session-request";
                var connectionFactory = new ConnectionFactory { HostName = config.HostName, Port = config.Port };
                var connectionClient = connectionFactory.CreateConnection();
                var connectionServer = connectionFactory.CreateConnection();

                using (var clientBus = new RabbitMqClientBus(exchangeName, queueName, connectionClient, messageSerializer))
                using (var serverBus = new RabbitMqServerBus(exchangeName, queueName, connectionServer, messageSerializer))
                {
                    clientBus.Init();
                    serverBus.Init();

                    clientBus.TimeoutValue = 1;

                    var request = new OpenSessionRequest { UserName = "login1" };

                    serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(req =>
                    {
                        Thread.Sleep(2000);
                        return new OpenSessionResponse { IsLogged = true };
                    });

                    await clientBus.Request<OpenSessionRequest, OpenSessionResponse>(request);
                };
            };
            Assert.That(testDelegate, Throws.TypeOf<TimeoutException>());
        }

        [Test]
        [Timeout(6000)]
        public async Task ShouldFirstAndThirdTimeout()
        {
            var config = new Configuration();
            var messageSerializer = new JsonMessageSerializer();
            var exchangeName = "session-exchange";
            var queueName = "session-request";
            var connectionFactory = new ConnectionFactory { HostName = config.HostName, Port = config.Port };
            var connectionClient = connectionFactory.CreateConnection();
            var connectionServer = connectionFactory.CreateConnection();

            using (var clientBus = new RabbitMqClientBus(exchangeName, queueName, connectionClient, messageSerializer))
            using (var serverBus = new RabbitMqServerBus(exchangeName, queueName, connectionServer, messageSerializer))
            {
                clientBus.Init();
                serverBus.Init();

                clientBus.TimeoutValue = 1;

                var request1 = new OpenSessionRequest { UserName = "login1" };
                var request2 = new OpenSessionRequest { UserName = "login2" };
                var request3 = new OpenSessionRequest { UserName = "login3" };

                serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(req =>
                {
                    if (req.UserName == "login1" || req.UserName == "login3") Thread.Sleep(2000);
                    return new OpenSessionResponse { IsLogged = true };
                });

                AsyncTestDelegate testDelegate1 = async () =>
                {
                    var response1 = await clientBus.Request<OpenSessionRequest, OpenSessionResponse>(request1);
                };

                var response2 = await clientBus.Request<OpenSessionRequest, OpenSessionResponse>(request2);

                AsyncTestDelegate testDelegate3 = async () =>
                {
                    var response3 = await clientBus.Request<OpenSessionRequest, OpenSessionResponse>(request3);
                };

                Assert.That(testDelegate1, Throws.TypeOf<TimeoutException>());
                Assert.That(response2.IsLogged, Is.True);
                Assert.That(testDelegate3, Throws.TypeOf<TimeoutException>());
            };
        }
    }
}
