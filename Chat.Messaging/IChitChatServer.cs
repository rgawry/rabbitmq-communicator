using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Messaging
{
    public interface IChitChatServer
    {
        Task<bool> TryLogin(string userName);
    }
}
