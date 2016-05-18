using System;

namespace Chat
{
    public class CommandProcessor : ICommandProcessor
    {
        private string _argument;
        private string _command;

        public void Process(string value)
        {
            var elements = value.Split(' ');
            if (elements.Length < 2)
            {
                _command = elements[0];
                return;
            }
            _command = elements[0];
            _argument = elements[1];
        }

        public string GetArgument()
        {
            return _argument;
        }

        public string GetCommand()
        {
            return _command;
        }
    }
}
