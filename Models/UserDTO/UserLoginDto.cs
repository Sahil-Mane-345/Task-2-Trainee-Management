
using System.ComponentModel.DataAnnotations;


namespace TraineeApi.Models.UserDTO;


public class UserLoginDto
{

    [Required(ErrorMessage = "UserName is required")]
    [StringLength(50, MinimumLength = 3 , ErrorMessage = "Username must be minimun 3 and maximum 5 in length.")]
    public required string UserName { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public required string Password { get; set; }
    
}