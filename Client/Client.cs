using Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface IClient
    {
        Task Start();
    }

    public class Client : IClient
    {
        private IChitChatServer _chitChatServer;

        public Client(IChitChatServer chitChatServer)
        {
            _chitChatServer = chitChatServer;
        }

        public async Task Start()
        {
            Console.WriteLine("Please provide login: ");
            var userName = Console.ReadLine();
            var isLogged = await _chitChatServer.TryLogin(userName);
            if(isLogged)
            {
                Console.WriteLine("Logged to chat.");
            }
            else
            {
                Console.WriteLine("Cant login to chat.");
            }
        }
    }
}
