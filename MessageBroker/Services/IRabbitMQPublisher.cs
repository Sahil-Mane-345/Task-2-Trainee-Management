namespace TraineeApi.MessageBroker.Services;

public interface IRabbitMQPublisher
{
    Task PublishMessageAsync<T>(T message, string queueName);
}