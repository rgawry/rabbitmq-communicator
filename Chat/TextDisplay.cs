using Castle.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Chat
{
    public class TextDisplay : IDisplay, IInitializable
    {
        private TextReader _input;
        private TextWriter _output;
        public event EventHandler<string> OneLine;

        public TextDisplay(TextReader input, TextWriter output)
        {
            _input = input;
            _output = output;
        }

        public void Initialize()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var line = await _input.ReadLineAsync();
                    var handler = OneLine;
                    if (handler != null) handler(this, line);
                }
            });
        }

        public async Task Print(string text)
        {
            await _output.WriteLineAsync(text);
        }
    }
}
