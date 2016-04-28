using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IChitChatServer
    {
        Task<bool> TryLogin(string userName);
    }

    public class ChitChatServer : IChitChatServer
    {
        private IBus _bus;

        public ChitChatServer(IBus bus)
        {
            _bus = bus;
        }

        public async Task<bool> TryLogin(string userName)
        {
            var request = new OpenSessionRequest { Login = userName, };
            var responseTask = await _bus.Request<OpenSessionRequest, OpenSessionResponse>(request);
            return responseTask.IsLogged;
        }
    }
}
