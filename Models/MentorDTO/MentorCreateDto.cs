using System.ComponentModel.DataAnnotations;

namespace TraineeApi.Models.MentorDTo;

public enum MentorStatus
{
    Active,

    Inactive
}

public class MentorCreateDto
{
    [Required(ErrorMessage = "Firstname is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Minimum 3 and Maximum 50 length is required")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lastname is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Minimum 3 and Maximum 50 length is required")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email Address is required")]
    [EmailAddress(ErrorMessage = "Email address must be valid")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Expertise is required")]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "Minimum 3 and Maximum 15 length is required")]
    public string Expertise { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status is required")]
    [EnumDataType(typeof(MentorStatus), ErrorMessage = "Invalid Status")]
    public string Status { get; set; } = string.Empty;
}