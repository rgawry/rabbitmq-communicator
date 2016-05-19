using NUnit.Framework;

namespace Chat
{
    [TestFixture]
    public class CommandProcessorTest
    {
        [Test]
        public void ShouldExtractCommandAndArgument()
        {
            var expectedCommand = "command";
            var expectedArgument = "argument";
            var line = expectedCommand + " " + expectedArgument;
            var commandProcessor = new CommandProcessor();

            var result = commandProcessor.Process(line);

            Assert.That(result.Item1, Is.EqualTo(expectedCommand));
            Assert.That(result.Item2, Is.EqualTo(expectedArgument));
        }

        [Test]
        public void ShouldExtractArgumentsAsOneString()
        {
            var expectedCommand = "command";
            var expectedArgument = "argument1 argument2";
            var line = expectedCommand + " " + expectedArgument;
            var commandProcessor = new CommandProcessor();

            var result = commandProcessor.Process(line);

            Assert.That(result.Item1, Is.EqualTo(expectedCommand));
            Assert.That(result.Item2, Is.EqualTo(expectedArgument));
        }

        [Test]
        public void ShouldExtractCommandOnly()
        {
            var expectedCommand = "command";
            var line = expectedCommand;
            var commandProcessor = new CommandProcessor();

            var result = commandProcessor.Process(line);

            Assert.That(result.Item1, Is.EqualTo(expectedCommand));
            Assert.That(result.Item2, Is.EqualTo(string.Empty));
        }
    }
}
