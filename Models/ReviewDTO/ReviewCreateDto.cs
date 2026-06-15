using System.ComponentModel.DataAnnotations;

namespace TraineeApi.Models.ReviewDTO;

public enum ReviewStatusType
{
    Accepted,
    ChangesRequired,
    Rejected
}

public class ReviewCreateDto
{
    [Required(ErrorMessage = "Submission Id is required")]
    public Guid? SubmissionId { get; set; } 

    [Required(ErrorMessage = "Mentor Id is required")]
    public Guid? MentorId { get; set; }

    public string Feedback { get; set; } = string.Empty;

    [Range(0, 10, ErrorMessage = "Score should be between 0 - 10")]
    public required int Score { get; set; } = 0;

    [Required(ErrorMessage = "Status is required")]
    [EnumDataType(typeof(ReviewStatusType), ErrorMessage = "Status should be valid")]
    public required string ReviewStatus { get; set; }

}