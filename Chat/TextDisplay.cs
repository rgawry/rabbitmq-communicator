using Castle.Core;
using System;
using System.IO;

namespace Chat
{
    public class TextDisplay : IDisplay, IInitializable
    {
        private TextReader _input;
        private TextWriter _output;
        public event EventHandler<TextInputEventArgs> OneLine;

        public TextDisplay(TextReader input, TextWriter output)
        {
            _input = input;
            _output = output;
        }

        public void Initialize()
        {
            ReadLine();
        }

        private async void ReadLine()
        {
            while (true)
            {
                var line = await _input.ReadLineAsync();
                var handler = OneLine;
                if (handler != null) handler(this, new TextInputEventArgs(line));
            }
        }

        public void Print(string text)
        {
            _output.WriteLineAsync(text);
        }
    }
}
