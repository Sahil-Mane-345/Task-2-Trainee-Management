namespace TraineeApi.Models.Entity;

public class TaskAssignment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required Guid TraineeId { get; set; }
    
    public Trainee Trainee { get; set; } = null!;

    public required Guid MentorId { get; set; }
    
    public Mentor Mentor { get; set; } = null!;

    public required Guid LearningTaskId { get; set; }
    
    public LearningTask LearningTask { get; set; } = null!;

    public required DateOnly AssignedDate { get; set; }

    public required DateOnly DueDate { get; set; }

    public required string Status { get; set; }

    public string Remarks { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

}