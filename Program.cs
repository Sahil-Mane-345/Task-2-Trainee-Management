using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using TraineeApi.Utility;
using System.Text.Json.Serialization;
using TraineeApi.Extensions;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCorsConfiguration();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddApiConfiguration();

var rabbitMQSection = builder.Configuration.GetSection("RabbitMQ");

builder.Services.AddSingleton( sp => new ConnectionFactory()
{
    HostName = rabbitMQSection["HostName"]!,
    Port = Convert.ToInt32(rabbitMQSection["Port"]),
    UserName = rabbitMQSection["UserName"]!,
    Password = rabbitMQSection["Password"]!,
    VirtualHost = rabbitMQSection["VirtualHost"]!,
});


builder.Services.AddStaticServices(builder.Configuration);

builder.Services.AddDbContext(builder.Configuration);

builder.Services.AddRedisContext(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks();

builder.Services.AddHealthChecksExtension(builder.Configuration);



builder.Services.AddHttpContextAccessor();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}


app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () =>
{
    return Results.Ok($"Welcome to Trainee Management System");
});

app.MapHealthChecks("/healthz");

await app.SeedDatabaseAsync();

app.Run();
