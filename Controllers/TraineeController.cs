using System;
using Microsoft.AspNetCore.Mvc;
using TraineeApi.Models.Entity;
using TraineeApi.Models;
using TraineeApi.Services.Interfaces;

namespace TraineeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class traineeController : ControllerBase{


    private readonly ITraineeService _traineeService;

    public traineeController(ITraineeService traineeService){
        _traineeService = traineeService;
    }

    [HttpGet(Name = "GetTrainees")]
    public IActionResult Get()
    {
        return Ok(_traineeService.GetAllTrainee());
        
    }

    [HttpPost(Name = "CreateTrainee")]
    public IActionResult Create(CreateTraineeRequest newTrainee){

        var t = _traineeService.CreateTrainee(newTrainee);
        return Created("/api/trainees",
            t
        );

    }

    [HttpGet("{id}", Name = "GetTrainee")]
    public IActionResult GetById(long id)
    {   
        return Ok(_traineeService.GetTraineeById(id));
    }

    // private TraineeResponse MapTraineeToDto(Trainee trainee){
    //     return new TraineeResponse{
    //         FirstName = trainee.FirstName,
    //         LastName = trainee.LastName,
    //         Email = trainee.Email,
    //         TechStack = trainee.TechStack,
    //         Status = trainee.Status,
    //         CreatedAt = trainee.CreatedAt,
    //         UpdatedAt = trainee.UpdatedAt
    //     };
    // }

    [HttpPut("{id}")]
    public IActionResult UpdateById(long id, UpdateTraineeRequest updateTrainee){
        var t = _traineeService.UpdateTrainee(id, updateTrainee);
        if (!t.Success){
            return NotFound(t);
        }else{
            return Ok(t);
        }
        ;
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteById(long id){
        bool t = _traineeService.DeleteTraineeById(id);
        if (t){
            return StatusCode(204, "Trainee deleted successfully");
        }else{
            return StatusCode(404, "Trainee Not Found");
        }
    }
} 