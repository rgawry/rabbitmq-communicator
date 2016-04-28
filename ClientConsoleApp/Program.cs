using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client;
using Common;

namespace ClientConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Factory.StartNew(() =>
            {
                new Client.Client(new ChitChatServer(new RabbitMqBus("session-exchange", "session-request"))).Start();
            });

            while (true) ;
        }
    }
}
