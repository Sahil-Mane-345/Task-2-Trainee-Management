using System.ComponentModel.DataAnnotations;

namespace TraineeApi.Models.TaskAssignmentDTO;

public class TaskAssignmentUpdateStatusDto
{
    [Required(ErrorMessage = "Status is required")]
    [EnumDataType(typeof(TaskAssignmentStatus), ErrorMessage = "Valid status is required")]
    public string? Status { get; set; }
}