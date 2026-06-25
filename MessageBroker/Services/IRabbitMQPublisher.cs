using TraineeApi.Models.SubmissionDTO;

namespace TraineeApi.MessageBroker.Services;

public interface IRabbitMQPublisher
{
    Task PublishFileMessageAsync(SubmissionProcessingRequestDto message, string queueName);
}