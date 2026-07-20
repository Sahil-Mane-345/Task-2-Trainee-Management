using Microsoft.AspNetCore.Mvc;
using TraineeApi.Models;
using TraineeApi.Services.Interfaces;
using TraineeApi.Models.TraineeDTO;
using Microsoft.AspNetCore.Authorization;

namespace TraineeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TraineeController : ControllerBase{


    private readonly ITraineeService _traineeService;

    public TraineeController(ITraineeService traineeService){
        _traineeService = traineeService;
    }

    [HttpGet()]
    public IActionResult GetAll(string search = "", int pageNumber = 1, int pageSize = 10, string status = "")
    {   

        return Ok(_traineeService.GetAllTrainee(search, pageNumber, pageSize, status));
        
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {  
        var r = await _traineeService.GetTraineeById(id);
        return Ok(r);
    }

    [HttpPost()]
    public async Task<IActionResult> Create(CreateTraineeRequest newTrainee){
        var r = await _traineeService.CreateTrainee(newTrainee);
        return CreatedAtAction(
            nameof(GetById),
            new { id = r?.Data?.Id},
            r
        );

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateById(Guid id, UpdateTraineeRequest updateTrainee){
        var r = await _traineeService.UpdateTrainee(id, updateTrainee);
        return Ok(r);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById(Guid id){
        bool t = await _traineeService.DeleteTraineeById(id);
        return NoContent();
        
    }
} 