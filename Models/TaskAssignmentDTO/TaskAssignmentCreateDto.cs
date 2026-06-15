using System.ComponentModel.DataAnnotations;

namespace TraineeApi.Models.TaskAssignmentDTO;


public enum TaskAssignmentStatus
{
    Assigned,
    InProgress,
    Submitted,
    Reviewed,
    Completed
}

public class TaskAssignmentCreateDto
{
    [Required(ErrorMessage = "TraineeId is required")]
    public Guid? TraineeId { get; set; }

    [Required(ErrorMessage = "MentorId is required")]
    public Guid? MentorId { get; set; }

    [Required(ErrorMessage = "LearningTaskId is required")]
    public Guid? LearningTaskId { get; set; }

    [Required(ErrorMessage = "AssignedDate is required")]
    public DateOnly AssignedDate { get; set; }

    [Required(ErrorMessage = "DueDate is required")]
    public DateOnly DueDate { get; set; }
    
    public string? Remarks { get; set; } = string.Empty;

}