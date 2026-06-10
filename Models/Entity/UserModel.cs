
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace TraineeApi.Models.Entity;



[Index(nameof(UserName), IsUnique = true)]
public class User
{
    [Key]
    public Guid  Id { get; set; } = Guid.NewGuid();

    public required string UserName { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public required string Role { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }
}