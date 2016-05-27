using Castle.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat
{
    public sealed class ChatClient : IInitializable
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
            _commands.Add(LOGIN_COMMAND);
            _display.OneLine += new EventHandler<TextInputEventArgs>(async (s, ea) => await OnOneLine(s, ea));
            PrintWelcome();
        }

        private async Task OnOneLine(object sender, TextInputEventArgs ea)
        {
            if (string.IsNullOrWhiteSpace(ea.Line)) return;

            var result = _commandProcessor.Process(ea.Line);
            var command = result.Item1;
            var argument = result.Item2;

            switch (command)
            {
                case LOGIN_COMMAND:
                    await LoginHandler(argument);
                    break;
                default:
                    _display.Print("Unknown command: " + command);
                    break;
            }
        }

        private void PrintWelcome()
        {
            _display.Print("Welcome to chat.");
            _display.Print("List of available commands:");
            PrintCommands();
        }

        private void PrintCommands()
        {
            foreach (var command in _commands)
            {
                _display.Print(" " + command);
            }
        }

        private async Task LoginHandler(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) _display.Print("Wrong argument.");

            var response = await _clientBus.Request(new OpenSessionRequest { UserName = value }).Response<OpenSessionResponse>();

            if (response.IsLogged)
            {
                _display.Print("Logged in as '" + value + "'");
            }
            else
            {
                _display.Print("Can't log in as '" + value + "'");
            }
        }
    }
}
