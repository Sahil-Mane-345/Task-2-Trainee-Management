
using System.ComponentModel.DataAnnotations;


namespace TraineeApi.Models.TraineeDTO;

public enum TraineeStatus{
    Active,
    Inactive,
    Completed
}

public class CreateTraineeRequest{

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "First Name must be between 3 and 50 characters.")]
    public required string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Last Name must be between 3 and 50 characters.")]
    public required string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email address.")]
    public required string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tech Stack is required.")]
    public required string TechStack { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status is required.")]
    [EnumDataType(typeof(TraineeStatus), ErrorMessage = "Invalid Status.")]
    public required string Status { get; set; } = string.Empty;

}