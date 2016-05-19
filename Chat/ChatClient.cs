using Castle.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat
{
    public class ChatClient : IInitializable
    {
        private const string LOGIN_COMMAND = "login";

        private List<string> _commands = new List<string>();
        private IClientBus _clientBus;
        private IDisplay _display;
        private ICommandProcessor _commandProcessor;

        public ChatClient(IClientBus clientBus, IDisplay display, ICommandProcessor commandProcessor)
        {
            _clientBus = clientBus;
            _display = display;
            _commandProcessor = commandProcessor;
        }

        public void Initialize()
        {
            Task.Run(async () =>
            {
                _commands.Add(LOGIN_COMMAND);
                _display.OneLine += new EventHandler<string>(async (s, e) => await OnOneLine(s, e));
                await PrintWelcome();
            }).Wait();
        }

        private async Task OnOneLine(object sender, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            var result = _commandProcessor.Process(value);
            var command = result.Item1;
            var argument = result.Item2;

            switch (command)
            {
                case LOGIN_COMMAND:
                    await LoginHandler(argument);
                    break;
                default:
                    await _display.Print("Unknown command: " + command);
                    break;
            }
        }

        private async Task PrintWelcome()
        {
            await _display.Print("Welcome to chat.");
            await _display.Print("List of available commands:");
            await PrintCommands();
        }

        private async Task PrintCommands()
        {
            foreach (var command in _commands)
            {
                await _display.Print(" " + command);
            }
        }

        private async Task LoginHandler(string value)
        {
            var response = await _clientBus.Request(new OpenSessionRequest { UserName = value }).Response<OpenSessionResponse>();
            await _display.Print((response.IsLogged ? "Logged in as '" : "Can't log in as '") + value + "'");
        }
    }
}
