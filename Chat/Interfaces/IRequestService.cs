namespace Chat
{
    public interface IRequestService<TRequest> : IServiceTag
    {
        void Handle(TRequest request);
    }
}
