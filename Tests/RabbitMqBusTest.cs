﻿using NUnit.Framework;
using RabbitMQ.Client;
using System;
using System.Reactive.Disposables;
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
        private CompositeDisposable _instanceDisposer;

        [TearDown]
        public void Dispose()
        {
            _instanceDisposer.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            _instanceDisposer = new CompositeDisposable();
        }

        [Test]
        public async Task ShouldReceiveSentMessage()
        {
            var serverBus = GetServerBus();
            using (var clientBus = GetClientBus())
            {
                serverBus.AddHandler<TestMessageA, TestMessageC>(req => new TestMessageC { Done = true });

                var response = await clientBus.Request<TestMessageA, TestMessageC>(new TestMessageA { Name = "login1" });

                Assert.That(response.Done, Is.True);
            }
        }

        [Test]
        public async Task ShouldMatchRequestWithResponse()
        {
            var serverBus = GetServerBus();
            using (var clientBus = GetClientBus())
            {
                serverBus.AddHandler<TestMessageA, TestMessageC>(req =>
                {
                    if (req.Name == "login1") Thread.Sleep(500);
                    return new TestMessageC { Done = req.Name == "login1" ? true : false };
                });

                var response1AsTask = clientBus.Request<TestMessageA, TestMessageC>(new TestMessageA { Name = "login1" });
                var response2AsTask = clientBus.Request<TestMessageA, TestMessageC>(new TestMessageA { Name = "login2" });

                await Task.WhenAll(response1AsTask, response2AsTask);

                Assert.That(response1AsTask.Result.Done, Is.True);
                Assert.That(response2AsTask.Result.Done, Is.False);
            }
        }

        [Test]
        public async Task ShouldMatchEachRequestTypeWithResponse()
        {
            var serverBus = GetServerBus();
            using (var clientBus = GetClientBus())
            {
                serverBus.AddHandler<TestMessageA, TestMessageC>(req => new TestMessageC { Done = true });
                serverBus.AddHandler<TestMessageB, TestMessageC>(req => new TestMessageC { Done = false });

                var response1AsTask = clientBus.Request<TestMessageA, TestMessageC>(new TestMessageA { Name = "login1" });
                var response2AsTask = clientBus.Request<TestMessageB, TestMessageC>(new TestMessageB { Name = "login2" });

                await Task.WhenAll(response1AsTask, response2AsTask);

                Assert.That(response1AsTask.Result.Done, Is.True);
                Assert.That(response2AsTask.Result.Done, Is.False);
            }
        }

        [Test]
        public void ShouldRequestTimeout()
        {
            AsyncTestDelegate testDelegate = async () =>
            {
                using (var clientBus = GetClientBus())
                {
                    clientBus.TimeoutValue = 0.1f;
                    await clientBus.Request<TestMessageA, TestMessageC>(new TestMessageA { Name = "login1" });
                };
            };
            Assert.That(testDelegate, Throws.TypeOf<TimeoutException>());
        }

        #region test-help
        private static Configuration config = new Configuration();

        private static IConnection CreateConnection()
        {
            var connectionFactory = new ConnectionFactory { HostName = config.HostName, Port = config.Port };
            return connectionFactory.CreateConnection();
        }

        private ClientBus GetClientBus()
        {
            var messageSerializer = new JsonMessageSerializer();
            var connectionClient = CreateConnection().DisposeWith(_instanceDisposer);
            var messagingProvider = new MessagingProvider(config.ExchangeRequestName, connectionClient);
            messagingProvider.Initialize();
            var clientBus = new ClientBus(messageSerializer, messagingProvider, config.QueueRequestName);
            clientBus.Initialize();

            return clientBus;
        }

        private ServerBus GetServerBus()
        {
            var messageSerializer = new JsonMessageSerializer();
            var connectionServer = CreateConnection().DisposeWith(_instanceDisposer);
            var messagingProvider = new MessagingProvider(config.ExchangeRequestName, connectionServer);
            messagingProvider.Initialize();
            var serverBus = new ServerBus(messageSerializer, messagingProvider, config.QueueRequestName);
            serverBus.Initialize();

            return serverBus;
        }
        #endregion
    }
}
