namespace Chat
{
    public interface IRabbitMqClientBusFactory
    {
        RabbitMqClientBus Create(IMessageSerializer messageSerializer);
    }
}
