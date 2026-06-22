using TraineeApi.Context;
using TraineeApi.Models.Entity;

namespace TraineeApi.Extensions;

public static class WebApplicationExtensions
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if( !context.Users.Any())
        {
            context.Users.Add( new User
            {
                UserName = "admin",
                Email = "admin@trainee.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123456"),
                Role = "Admin"
            });

            context.SaveChanges();
        }
    }
}