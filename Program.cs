using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using TraineeApi.Utility;
using System.Text.Json.Serialization;
using TraineeApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCorsConfiguration();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddApiConfiguration();


builder.Services.AddStaticServices();

builder.Services.AddDbContext(builder.Configuration);

builder.Services.AddRedisContext(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();



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


await app.SeedDatabaseAsync();

app.Run();
