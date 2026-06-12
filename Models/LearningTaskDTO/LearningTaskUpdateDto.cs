using System.ComponentModel.DataAnnotations;

namespace TraineeApi.Models.LearningTaskDTO;

public class LearningTaskUpdateDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Minimum 3 and Maximum 50 length is required")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Minimum 3 and Maximum 50 length is required")]
    public string Description { get; set; } =string.Empty;

    [Required(ErrorMessage = "Expected Tech Stack is required")]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "Minimum 3 and Maximum 15 length is required")]
    public string ExpectedTechStack { get; set; } = string.Empty;

    [Required(ErrorMessage = "Duedate is required")]
    public DateOnly DueDate { get; set; } = DateOnly.MinValue;

    [Required(ErrorMessage = "Status is required")]
    [EnumDataType(typeof(LearningTaskStatus), ErrorMessage = "Learning Task Status must be valid")]
    public string Status { get; set; } = string.Empty;
}