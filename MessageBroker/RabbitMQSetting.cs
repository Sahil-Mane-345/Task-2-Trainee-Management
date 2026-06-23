namespace TraineeApi.MessageBroker;

public class RabbitMQSetting
{
    public string HostName { get; set; } = string.Empty;

    public int Port { get; set; } 

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string VirtualHost { get; set; } = string.Empty;
}

public static class RabbitMQQueues
{
    public const string SubmissionProcessing = "submission-processing";
}