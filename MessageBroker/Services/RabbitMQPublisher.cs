using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using TraineeApi.Context;
using TraineeApi.MessageBroker.Constants;
using TraineeApi.MessageBroker.Entity;
using TraineeApi.Models.Entity;
using TraineeApi.Models.SubmissionDTO;

namespace TraineeApi.MessageBroker.Services;

public class RabbitMQPublisher : IRabbitMQPublisher
{
    private readonly ConnectionFactory _connection;
    private readonly ILogger<RabbitMQPublisher> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RabbitMQPublisher(ConnectionFactory connection, ILogger<RabbitMQPublisher> logger, IServiceScopeFactory serviceScopeFactory )
    {
        _logger = logger;
        _connection = connection;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task PublishFileMessageAsync(SubmissionProcessingRequestDto message, string queueName)
    {
        

        try
        {
            using var connection = await _connection.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
            queue: RabbitMQConstants.SubmissionProcessingQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>{
                ["x-dead-letter-exchange"] = RabbitMQConstants.DeadLetterExchange,
                ["x-dead-letter-routing-key"] = RabbitMQConstants.DeadLetterRoutingKey
            }
        );
            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            var properties = new BasicProperties
            {
                Persistent = true,
                CorrelationId = Guid.NewGuid().ToString(),
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };
            
            ProcessingJob processingJob = new()
            {
                CorrelationId = properties.CorrelationId,
                SubmissionFileId = message.SubmissionFileId,
                MessageId = properties.MessageId,
                Status = "Queued"
            };

            using var scope = _serviceScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await Task.Run( async () => await channel.BasicPublishAsync(exchange: "", routingKey : queueName, body: body, basicProperties: properties, mandatory: false));
            await db.ProcessingJobs.AddAsync(processingJob);
            await db.SaveChangesAsync();

        }
        catch (System.Exception)
        {
            _logger.LogError("Rabbitmq is not working");
        }
    }
}