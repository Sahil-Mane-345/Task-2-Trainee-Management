
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
    public string? FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Last Name must be between 3 and 50 characters.")]
    public string? LastName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email address.")]
    public string? Email { get; set; } = null!;

    [Required(ErrorMessage = "Tech Stack is required.")]
    public  string? TechStack { get; set; } = null!;

    [Required(ErrorMessage = "Status is required.")]
    [EnumDataType(typeof(TraineeStatus), ErrorMessage = "Invalid Status.")]
    public string? Status { get; set; } = null!;

}