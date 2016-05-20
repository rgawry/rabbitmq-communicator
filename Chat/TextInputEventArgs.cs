using System;

namespace Chat
{
    public class TextInputEventArgs : EventArgs
    {
        public string Line { get; set; }

        public TextInputEventArgs(string line)
        {
            Line = line;
        }
    }
}
