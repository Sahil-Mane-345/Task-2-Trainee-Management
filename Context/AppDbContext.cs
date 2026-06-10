using Microsoft.EntityFrameworkCore;
using TraineeApi.Models.Entity;

namespace TraineeApi.Context;

public class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Trainee> Trainees { get; set; }
}
