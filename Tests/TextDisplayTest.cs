using NSubstitute;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Chat
{
    [TestFixture]
    public class TextDisplayTest
    {
        [Test]
        public void ShouldAcceptInputWhilePrinting()
        {
            var testInput = "test_input";
            var reader = Substitute.For<TextReader>();
            var writer = Substitute.For<TextWriter>();
            var display = new TextDisplay(reader, writer);
            var handler = new EventHandler<string>((s, e) => { Assert.That(e, Is.EqualTo(testInput)); });

            try
            {
                Task.Run(async () =>
                {
                    while (true)
                    {
                        await display.Print("test");
                    }
                });

                reader.ReadLineAsync().Returns(testInput);

                display.OneLine += handler;
            }
            finally
            {
                display.OneLine -= handler;
            }
        }
    }
}
