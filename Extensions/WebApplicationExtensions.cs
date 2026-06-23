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

        if( !context.Users.Any())
        {
            context.Users.Add( new User
            {
                UserName = "admin",
                Email = "admin@trainee.com",
                PasswordHash = PasswordHashing.HashPassword("Admin@123456"),
                Role = "Admin"
            });

            await context.SaveChangesAsync();
        }
    }
}