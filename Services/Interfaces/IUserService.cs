using TraineeApi.Models.UserDTO;

namespace TraineeApi.Services.Interfaces;

public interface IUserService
{
    UserResponseDto LoginUser(UserLoginDto userLoginDto);
}