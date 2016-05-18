namespace Chat
{
    public interface ICommandProcessor
    {
        void Process(string value);
        string GetArgument();
        string GetCommand();
    }
}
