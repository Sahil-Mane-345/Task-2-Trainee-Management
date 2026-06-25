using TraineeApi.Models.SubmissionDTO;

namespace TraineeApi.MessageBroker.Services;

public interface IRabbitMQPublisher
{
    Task PublishFileMessageAsync<T>(T message, string queueName, string CorrelationId, string MessageId);
}