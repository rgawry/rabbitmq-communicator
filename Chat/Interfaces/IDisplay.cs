using System;
using System.Threading.Tasks;

namespace Chat
{
    public interface IDisplay
    {
        void Print(string text);
        event EventHandler<TextInputEventArgs> OneLine;
    }
}
