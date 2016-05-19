using NSubstitute;
using NUnit.Framework;
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

            Task.Run(async () =>
            {
                while (true)
                {
                    await display.Print("test");
                }
            });

            reader.ReadLineAsync().Returns(testInput);

            display.OneLine += (s, e) => { Assert.That(e, Is.EqualTo(testInput)); };
        }
    }
}
