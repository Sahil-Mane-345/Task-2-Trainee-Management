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

    public DbSet<Mentor> Mentors { get; set; }

    public DbSet<LearningTask> LearningTasks { get; set; }
}
