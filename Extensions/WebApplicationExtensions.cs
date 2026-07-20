using Microsoft.EntityFrameworkCore;
using TraineeApi.Context;
using TraineeApi.Models.Entity;
using TraineeApi.Utility;

namespace TraineeApi.Extensions;

public static class WebApplicationExtensions
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Database.Migrate();

        if( !context.Users.Any())
        {
            context.Users.Add( new User
            {
                UserName = Environment.GetEnvironmentVariable("ADMIN_USERNAME")!,
                Email = Environment.GetEnvironmentVariable("ADMIN_EMAIL")!,
                PasswordHash = PasswordHashing.HashPassword(Environment.GetEnvironmentVariable("ADMIN_PASSWORD")!),
                Role = "Admin"
            });

            await context.SaveChangesAsync();
        }
    }
}