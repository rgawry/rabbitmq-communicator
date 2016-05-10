namespace Chat
{
    public interface IRabbitMqServerBusFactory
    {
        RabbitMqServerBus Create(IMessageSerializer messageSerializer);
    }
}
