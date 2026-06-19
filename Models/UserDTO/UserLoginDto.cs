
using System.ComponentModel.DataAnnotations;


namespace TraineeApi.Models.UserDTO;


public class UserLoginDto
{

    [Required(ErrorMessage = "UserName is required")]
    [StringLength(50, MinimumLength = 3 , ErrorMessage = "Username must be minimun 3 and maximum 5 in length.")]
    public required string UserName { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [RegularExpression(
        @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character."
    )]
    public required string Password { get; set; }
    
}