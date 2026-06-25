using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using TraineeApi.Context;
using TraineeApi.MessageBroker.Constants;
using TraineeApi.MessageBroker.Entity;
using TraineeApi.Models.Entity;
using TraineeApi.Models.SubmissionDTO;
using TraineeApi.Utility.Exception;

namespace TraineeApi.MessageBroker.Services;

public class RabbitMQPublisher : IRabbitMQPublisher
{
    private readonly ConnectionFactory _connection;
    private readonly ILogger<RabbitMQPublisher> _logger;

    public RabbitMQPublisher(ConnectionFactory connection, ILogger<RabbitMQPublisher> logger )
    {
        _logger = logger;
        _connection = connection;
    }

    public async Task PublishFileMessageAsync<T>(T message, string queueName, string CorrelationId, string MessageId)
    {
        

        try
        {
            using var connection = await _connection.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
            queue: queueName,
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
                CorrelationId = CorrelationId,
                MessageId = MessageId,
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };
            
            await Task.Run( async () => await channel.BasicPublishAsync(exchange: "", routingKey : queueName, body: body, basicProperties: properties, mandatory: false));

        }
        catch (System.Exception)
        {
            _logger.LogError("Rabbitmq is not working");
        }
    }
}