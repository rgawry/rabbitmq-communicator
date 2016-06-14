namespace Chat
{
    public interface IRequestResponseService<TRequest, TResponse> : IServiceTag
    {
        TResponse Handle(TRequest request);
    }
}
