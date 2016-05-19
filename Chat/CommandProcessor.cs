using System;

namespace Chat
{
    public class CommandProcessor : ICommandProcessor
    {

        public Tuple<string, string> Process(string value)
        {
            var elements = value.Split(' ');
            if (elements.Length < 2)
            {
                return new Tuple<string, string>(elements[0], string.Empty);
            }
            return new Tuple<string, string>(elements[0], elements[1]);
        }
    }
}
