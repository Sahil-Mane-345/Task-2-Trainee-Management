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

    [HttpGet()]
    public async Task<IActionResult> GetAll(string search = "")
    {
        return Ok(await _traineeService.GetAllTrainee(search));
        
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {  
        var r = await _traineeService.GetTraineeById(id);
        if(!r.success){
            return NotFound(r);
        }
        return Ok(r);
    }

    [HttpPost()]
    public async Task<IActionResult> Create(CreateTraineeRequest newTrainee){
        var r = await _traineeService.CreateTrainee(newTrainee);
        return CreatedAtAction(
            nameof(GetById),
            new { id = r?.data?.Id},
            r
        );

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
    public async Task<IActionResult> UpdateById(long id, UpdateTraineeRequest updateTrainee){
        var r = await _traineeService.UpdateTrainee(id, updateTrainee);
        if (!r.success){
            return NotFound(r);
        }else{
            return Ok(r);
        }
        ;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById(long id){
        bool t = await _traineeService.DeleteTraineeById(id);
        if (t){
            return NoContent();
        }else{
            return NotFound(new {message = $"Trainee with Id : {id} not found "});
        }
    }
} 