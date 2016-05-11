namespace Chat
{
    public interface IRabbitMqClientBusFactory
    {
        RabbitMqClientBus Create();
    }
}
