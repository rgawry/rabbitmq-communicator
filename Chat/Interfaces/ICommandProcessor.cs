using System;

namespace Chat
{
    public interface ICommandProcessor
    {
        Tuple<string, string> Process(string value);
    }
}
