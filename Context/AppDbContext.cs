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

    public DbSet<TaskAssignment> TaskAssignments { get; set; }

    public DbSet<Review> Reviews { get; set; }

    public DbSet<Submission> Submissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    foreach (var entity in modelBuilder.Model.GetEntityTypes())
    {
        foreach (var property in entity.GetProperties())
        {
            if (property.ClrType == typeof(Guid))
            {
                property.SetColumnType("char(36)");
                property.SetCollation("utf8mb4_0900_ai_ci");
            }
        }
    }
}
}
