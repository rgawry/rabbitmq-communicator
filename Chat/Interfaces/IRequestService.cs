namespace Chat
{
    public interface IRequestService<TRequest>
    {
        void Handle(TRequest request);
    }
}
