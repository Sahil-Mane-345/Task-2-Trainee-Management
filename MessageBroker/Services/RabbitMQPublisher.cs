using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace TraineeApi.MessageBroker.Services;

public class RabbitMQPublisher : IRabbitMQPublisher
{
    private readonly ConnectionFactory _connection;
    private readonly ILogger<RabbitMQPublisher> _logger;

    public RabbitMQPublisher(ConnectionFactory connection, ILogger<RabbitMQPublisher> logger)
    {
        _logger = logger;
        _connection = connection;
        
    }

    public async Task PublishMessageAsync<T>(T message, string queueName)
    {
        

        try
        {
            using var connection = await _connection.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await Task.Run( async () => await channel.BasicPublishAsync(exchange: "", routingKey : queueName, body: body, basicProperties: properties, mandatory: false));
        }
        catch (System.Exception)
        {
            _logger.LogInformation("Rabbitmq is not working");
        }
    }
}