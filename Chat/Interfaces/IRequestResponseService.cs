namespace Chat
{
    public interface IRequestResponseService<TRequest, TResponse>
    {
        TResponse Handle(TRequest request);
    }
}
