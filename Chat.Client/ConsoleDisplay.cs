using System;

namespace Chat
{
    public class ConsoleDisplay : IDisplay
    {
        public string OnKeyboard()
        {
            return Console.ReadLine();
        }

        public void Print(string text)
        {
            Console.WriteLine(text);
        }
    }
}
