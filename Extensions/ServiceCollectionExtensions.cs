using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using StackExchange.Redis;
using TraineeApi.Context;
using TraineeApi.MessageBroker;
using TraineeApi.MessageBroker.Services;
using TraineeApi.Services;
using TraineeApi.Services.Interfaces;
using TraineeApi.Services.Redis;


namespace TraineeApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStaticServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQSetting>(configuration.GetSection("RabbitMQ"));

        services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();

        services.AddScoped<ITraineeService, TraineeDbService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMentorService, MentorService>();
        services.AddScoped<ILearningTaskService, LearningTaskService>();
        services.AddScoped<ITaskAssignmentService, TaskAssignmentService>();
        services.AddScoped<ISubmissionService, SubmissionService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IProcessingJobService, ProcessingJobService>();

        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        services.AddSingleton<IRedisService, RedisService>();

        return services;
    }

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {

        string connectionStringMySQL = configuration.GetConnectionString("DefaultConnection")!;
        ServerVersion serverVersion = ServerVersion.AutoDetect(connectionStringMySQL);

        services.AddDbContext<AppDbContext>( opt =>
        {
            opt.UseMySql(connectionStringMySQL, serverVersion);
        });

        return services;
    }

    public static IServiceCollection AddRedisContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<Program>>();
            var redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { configuration.GetConnectionString("RedisConnection")! },
                AbortOnConnectFail = false,
                ConnectRetry = 3,
                ReconnectRetryPolicy = new ExponentialRetry(5000),
                
            });
            redis.ConnectionFailed += (sender, args) =>
                logger.LogError(args.Exception, "Redis Connection failed. Endpoint: {Endpoint}", args.EndPoint);

            redis.ConnectionRestored += (sender, args) =>
                logger.LogInformation(args.Exception, "Redis Connection Restored. Endpoint: {Endpoint}", args.EndPoint);

            redis.ErrorMessage += (sender, args) =>
                logger.LogError("Redis Error. Endpoint: {Endpoint}", args.EndPoint);

            redis.ConfigurationChanged += (sender, args) =>
                logger.LogInformation("Redis Connection Configuration Changed. Endpoint: {Endpoint}", args.EndPoint);
            
            return redis;
        });

        return services;
    }

    public static IServiceCollection AddRabbitMQContext(this IServiceCollection services, IConfiguration configuration)
    {
        IConfigurationSection rabbitMQSection = configuration.GetSection("RabbitMQ");

        services.AddSingleton( sp => new ConnectionFactory()
        {
            HostName = rabbitMQSection["HostName"]!,
            Port = Convert.ToInt32(rabbitMQSection["Port"]),
            UserName = rabbitMQSection["UserName"]!,
            Password = rabbitMQSection["Password"]!,
            VirtualHost = rabbitMQSection["VirtualHost"]!,

            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10),

            TopologyRecoveryEnabled = true
        });
        return services;
    }

    public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    {
        services.AddControllers( options =>
        {
            options.ModelMetadataDetailsProviders.Add(
                new SystemTextJsonValidationMetadataProvider()
            );
        });

        services.AddControllers().AddJsonOptions( options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull;
        });


        return services;
    }

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors( options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader();
                }
            );
        });

        return services;
    }

    public static IServiceCollection AddHealthChecksExtension(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddHealthChecks().AddMySql(
            configuration.GetConnectionString("DefaultConnection")!,
            name: "MySQL"
        ).AddRedis(
            configuration.GetConnectionString("RedisConnection")!,
            name: "Redis"
        ).AddRabbitMQ(
            async sp => await sp.GetRequiredService<ConnectionFactory>().CreateConnectionAsync(),
            name: "RabbitMQ"
        );
        return services;
    }
}