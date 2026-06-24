using Microsoft.AspNetCore.Mvc;
using TraineeApi.Context;
using TraineeApi.Models.UserDTO;
using TraineeApi.Models.Entity;
using TraineeApi.Services.Interfaces;
namespace TraineeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public IActionResult LoginAuth(UserLoginDto login)
    {
        try
        {
            var res = _userService.LoginUser(login);
            return Ok(res);
        }catch(Exception e)
        {
            return Unauthorized(e.Message);
        }
        
    }
}