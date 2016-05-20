using System;
using System.Threading.Tasks;

namespace Chat
{
    public interface IDisplay
    {
        Task Print(string text);
        event EventHandler<TextInputEventArgs> OneLine;
    }
}
