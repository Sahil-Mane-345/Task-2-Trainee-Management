namespace TraineeApi.Models.Entity;

public class Review
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required Guid SubmissionId { get; set; } 

    public required Guid MentorId { get; set; }

    public required string Feedback { get; set; }

    public required int Score { get; set; }

    public required string ReviewStatus { get; set; }

    public required DateOnly ReviewdDate { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public Submission Submission { get; set; } = null!;

    public Mentor Mentor { get; set; } = null!;

}