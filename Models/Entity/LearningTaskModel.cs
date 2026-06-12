namespace TraineeApi.Models.Entity;

public class LearningTask()
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Title { get; set; }

    public required string Description { get; set; }

    public required string ExpectedTechStack { get; set; }

    public required DateOnly DueDate { get; set; }

    public required string Status { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }
}