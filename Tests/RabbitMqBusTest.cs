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
    //[Timeout(2000)]
    public class RabbitMqBusTest
    {
        private static IConnection CreateConnection()
        {
            var config = new Configuration();
            var connectionFactory = new ConnectionFactory { HostName = config.HostName, Port = config.Port };
            return connectionFactory.CreateConnection();
        }

        private static RabbitMqClientBus GetClientBus()
        {
            var messageSerializer = new JsonMessageSerializer();
            var exchangeName = "session-exchange";
            var queueName = "session-request";
            var connectionClient = CreateConnection();

            return new RabbitMqClientBus(exchangeName, queueName, connectionClient, messageSerializer);
        }

        private static RabbitMqServerBus GetServerBus()
        {
            var messageSerializer = new JsonMessageSerializer();
            var exchangeName = "session-exchange";
            var queueName = "session-request";
            var connectionServer = CreateConnection();

            return new RabbitMqServerBus(exchangeName, queueName, connectionServer, messageSerializer);
        }

        [Test]
        public async Task ShouldReceiveSentMessage()
        {

            using (var clientBus = GetClientBus())
            using (var serverBus = GetServerBus())
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
            using (var clientBus = GetClientBus())
            using (var serverBus = GetServerBus())
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
        public async Task ShouldMatchEachRequestTypeWithResponse()
        {
            using (var clientBus = GetClientBus())
            using (var serverBus = GetServerBus())
            {
                clientBus.Initialize();
                serverBus.Initialize();

                var request1 = new TestMessageA { Name = "login1" };
                var request2 = new TestMessageB { Name = "login2" };

                serverBus.AddHandler<TestMessageA, TestMessageC>(req => new TestMessageC { Done = true });
                serverBus.AddHandler<TestMessageB, TestMessageC>(req => new TestMessageC { Done = true });

                var response1AsTask = clientBus.Request<TestMessageA, TestMessageC>(request1);
                var response2AsTask = clientBus.Request<TestMessageB, TestMessageC>(request2);

                await Task.WhenAll(response1AsTask, response2AsTask);

                Assert.That(response1AsTask.Result.Done, Is.True);
                Assert.That(response2AsTask.Result.Done, Is.True);
            }
        }

        [Test]
        [Timeout(3000)]
        public void ShouldTimeout()
        {
            AsyncTestDelegate testDelegate = async () =>
            {
                using (var clientBus = GetClientBus())
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
