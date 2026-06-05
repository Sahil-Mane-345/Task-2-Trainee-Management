using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TraineeApi.Models;

namespace TraineeApi.Models;


public class UpdateTraineeRequest{

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "First Name must be between 3 and 50 characters.")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Last Name must be between 3 and 50 characters.")]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email address.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Tech Stack is required.")]
    public required string TechStack { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [EnumDataType(typeof(TraineeStatus), ErrorMessage = "Invalid Status.")]
    public required string Status { get; set; }

}