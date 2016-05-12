using System.Threading.Tasks;

namespace Chat
{
    public interface IChatClient
    {
        Task TryLogIn();
    }
}