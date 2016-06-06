namespace Chat
{
    public interface IService<TRequest, TResponse>
    {
        TResponse Handle(TRequest request);
    }
}
