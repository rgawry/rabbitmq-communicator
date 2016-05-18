using Castle.Core;
using System;
using System.Threading.Tasks;

namespace Chat
{
    public class ChatClient : IInitializable
    {
        private IClientBus _clientBus;
        private IDisplay _display;

        public ChatClient(IClientBus clientBus, IDisplay display)
        {
            _clientBus = clientBus;
            _display = display;
        }

        public void Initialize()
        {
            Task.Run(async () =>
            {
                _display.OneLine += new EventHandler<string>(async (s, e) => await OnOneLine(s, e));
                await PrintWelcome();
            }).Wait();
        }

        private async Task OnOneLine(object sender, string value)
        {
            if (value.Contains("login"))
            {
                await LoginHandler(value.Split(' ')[1]);
            }
            else
            {
                await _display.Print("Unknown command");
            }
        }

        private async Task PrintWelcome()
        {
            await _display.Print("Welcome to chat.");
            await _display.Print("List of available commands: ");
            await PrintCommands();
        }

        private async Task PrintCommands()
        {
            await _display.Print(" login <username>");
        }

        private async Task LoginHandler(string value)
        {
            var response = await _clientBus.Request(new OpenSessionRequest { UserName = value }).Response<OpenSessionResponse>();

            if (response.IsLogged)
            {
                await _display.Print("Logged in as '" + value + "'");
            }
            else
            {
                await _display.Print("Can't log in as '" + value + "'");
            }
        }
    }
}
