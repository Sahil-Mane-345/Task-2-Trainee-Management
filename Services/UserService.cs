using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TraineeApi.Context;
using TraineeApi.Models.UserDTO;
using TraineeApi.Models.Entity;
using TraineeApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TraineeApi.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    private readonly ILogger<UserService> _logger;
    private readonly int _expiresIn;
    public UserService(AppDbContext context, IConfiguration configuration, ILogger<UserService> logger)
    {
        _context = context;
        _configuration = configuration;
        _expiresIn = Convert.ToInt32(_configuration["JWT:ExpiresIn"]);
        _logger = logger;
    }

    public UserResponseDto LoginUser(UserLoginDto userLoginDto)
    {
        User? existing = _context.Users.FirstOrDefault( u => u.UserName == userLoginDto.UserName );
        if(existing == null)
        {
            throw new UnauthorizedAccessException("Invalid Credentials");
        }
        bool verifyPass = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, existing.PasswordHash);
        if (!verifyPass)
        {
            throw new UnauthorizedAccessException("Invalid Credentials.");
        }
        string token = GenerateToken(existing.Id, existing.UserName, existing.Role.ToString());

        UserResponseDto res = new UserResponseDto
        {
            Token = token,
            ExpiresIn = _expiresIn,
            User = new UserRes
            {
                Id = existing.Id,
                UserName = existing.UserName,
                Role = existing.Role
            }
        };
        _logger.LogInformation($"User with Id : {existing.Id} UserName : {existing.UserName} Role : {existing.Role} logged in");
        return res;
    }

    private string GenerateToken(Guid Id, string userName, string Role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Role, Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.Now.AddSeconds(Convert.ToInt32(_expiresIn)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}