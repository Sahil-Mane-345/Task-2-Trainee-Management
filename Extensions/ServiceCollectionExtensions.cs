using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
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
        services.AddStackExchangeRedisCache( options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
            options.InstanceName = "TraineeManagementApi";
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