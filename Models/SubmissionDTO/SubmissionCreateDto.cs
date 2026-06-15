using System.ComponentModel.DataAnnotations;

namespace TraineeApi.Models.SubmissionDTO;

public class SubmissionCreateDto
{
    [Required(ErrorMessage = "Task Assignment Id is required")]
    public Guid? TaskAssignmentId { get; set; }

    [Required(ErrorMessage = "Submission URL is required")]
    [Url(ErrorMessage = "Provide valid URL")]
    public  string? SubmissionUrl { get; set; }

    public string Notes { get; set; } = string.Empty;

}