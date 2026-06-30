using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using TraineeApi.MessageBroker.Constants;

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
        catch (BrokerUnreachableException ex)
        {
            _logger.LogError(ex, "Rabbitmq is not working");
        }
    }
}