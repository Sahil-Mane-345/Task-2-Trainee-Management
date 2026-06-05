using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TraineeApi.Models;

public class TraineeResponse{

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Email { get; set; }

    public required string TechStack { get; set; }

    public required string Status { get; set; }

    public required DateTime CreatedAt { get; set; } 

    public DateTime? UpdatedAt { get; set; }
}