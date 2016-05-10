using NUnit.Framework;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chat
{
        public class TestMessageA { public string Name { get; set; } }
        public class TestMessageB { public string Name { get; set; } }
        public class TestMessageC { public bool Done { get; set; } }

    [TestFixture]
    [Timeout(2000)]
    public class RabbitMqBusTest
    {
        private static IConnection CreateConnection()
        {
            var config = new Configuration();
            var connectionFactory = new ConnectionFactory { HostName = config.HostName, Port = config.Port };
            return connectionFactory.CreateConnection();
        }

        [Test]
        public async Task ShouldReceiveSentMessage()
        {
            var messageSerializer = new JsonMessageSerializer();
            var exchangeName = "session-exchange";
            var queueName = "session-request";
            var connectionClient = CreateConnection();
            var connectionServer = CreateConnection();

            using (var clientBus = new RabbitMqClientBus(exchangeName, queueName, connectionClient, messageSerializer))
            using (var serverBus = new RabbitMqServerBus(exchangeName, queueName, connectionServer, messageSerializer))
            {
                clientBus.Initialize();
                serverBus.Initialize();

                var request = new TestMessageA { Name = "login1" };

                serverBus.AddHandler<TestMessageA, TestMessageC>(req => new TestMessageC { Done = true });

                var response = await clientBus.Request<TestMessageA, TestMessageC>(request);

                Assert.That(response.Done, Is.True);
            }
        }

        [Test]
        public async Task ShouldMatchRequestWithResponse()
        {
            var messageSerializer = new JsonMessageSerializer();
            var exchangeName = "session-exchange";
            var queueName = "session-request";
            var connectionClient = CreateConnection();
            var connectionServer = CreateConnection();

            using (var clientBus = new RabbitMqClientBus(exchangeName, queueName, connectionClient, messageSerializer))
            using (var serverBus = new RabbitMqServerBus(exchangeName, queueName, connectionServer, messageSerializer))
            {
                clientBus.Initialize();
                serverBus.Initialize();

                var request1 = new TestMessageA { Name = "login1" };
                var request2 = new TestMessageA { Name = "login2" };

                serverBus.AddHandler<TestMessageA, TestMessageC>(req =>
                {
                    if (req.Name == "login1") Thread.Sleep(500);
                    return new TestMessageC { Done = req.Name == "login1" ? true : false };
                });

                var response1AsTask = clientBus.Request<TestMessageA, TestMessageC>(request1);
                var response2AsTask = clientBus.Request<TestMessageA, TestMessageC>(request2);

                await Task.WhenAll(response1AsTask, response2AsTask);

                Assert.That(response1AsTask.Result.Done, Is.True);
                Assert.That(response2AsTask.Result.Done, Is.False);
            }
        }

        [Test]
        [Timeout(3000)]
        public void ShouldTimeout()
        {
            AsyncTestDelegate testDelegate = async () =>
            {
                var messageSerializer = new JsonMessageSerializer();
                var exchangeName = "session-exchange";
                var queueName = "session-request";
                var connectionClient = CreateConnection();

                using (var clientBus = new RabbitMqClientBus(exchangeName, queueName, connectionClient, messageSerializer))
                {
                    clientBus.Initialize();
                    clientBus.TimeoutValue = 0.1f;
                    var request = new TestMessageA { Name = "login1" };
                    await clientBus.Request<TestMessageA, TestMessageC>(request);
                };
            };
            Assert.That(testDelegate, Throws.TypeOf<TimeoutException>());
        }
    }
}
