

namespace TraineeApi.Models.UserDTO;

public class UserRes
{
    public Guid Id { get; set; }

    public string? UserName { get; set; }

    public string? Role { get; set; }
}


public class UserResponseDto
{
    public required string Token { get; set; }

    public required int ExpiresIn { get; set; }

    public required UserRes User { get; set; }

}