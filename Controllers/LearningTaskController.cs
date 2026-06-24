using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeApi.Models.LearningTaskDTO;
using TraineeApi.Services.Interfaces;

namespace TraineeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class LearningTaskController : ControllerBase
{
    private readonly ILearningTaskService _learningTaskService;

    public LearningTaskController(ILearningTaskService learningTaskService)
    {
        _learningTaskService = learningTaskService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _learningTaskService.GetAllLearningTasks());
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> GetById(Guid Id)
    {
        var r = await _learningTaskService.GetLearningTaskById(Id);
        return Ok(r);
    }

    [HttpPost()]
    public async Task<IActionResult> Create(LearningTaskCreateDto learningTaskCreateDto)
    {
        var r = await _learningTaskService.CreateLearningTask(learningTaskCreateDto);
        return CreatedAtAction(
            nameof(GetById),
            new { Id = r?.Data?.Id},
            r
        );
    }
    

    [HttpPut("{Id}")]
    public async Task<IActionResult> UpdateById(Guid Id, LearningTaskUpdateDto learningTaskUpdateDto)
    {
        var r = await _learningTaskService.UpdateLearningTask(Id, learningTaskUpdateDto);
        return Ok(r);
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeleteById(Guid Id){
        bool t = await _learningTaskService.DeleteLearningTaskById(Id);
        return NoContent();
    }
    
}