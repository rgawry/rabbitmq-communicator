using System.Threading.Tasks;

namespace Chat
{
    public class ChatClient
    {
        private IClientBus _clientBus;
        private IDisplay _display;

        public ChatClient(IClientBus clientBus, IDisplay display)
        {
            _clientBus = clientBus;
            _display = display;
        }

        public async Task TryLogIn()
        {
            while (true)
            {
                _display.Print("Login: ");
                var login = _display.OnKeyboard();
                var response = await _clientBus.Request(new OpenSessionRequest { UserName = login }).Response<OpenSessionResponse>();

                if (response.IsLogged)
                {
                    _display.Print("Logged in as '" + login + "'");
                    break;
                }
                else
                {
                    _display.Print("Cant log in as '" + login + "'");
                }
            }
        }
    }
}
