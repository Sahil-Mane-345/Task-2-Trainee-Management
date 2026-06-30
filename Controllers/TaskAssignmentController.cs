
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.TaskAssignmentDTO;
using TraineeApi.Services.Interfaces;

namespace TraineeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class TaskAssignmentController : ControllerBase
{
    private readonly ITaskAssignmentService _taskAssignmentService;

    public TaskAssignmentController(ITaskAssignmentService taskAssignmentService)
    {
        _taskAssignmentService = taskAssignmentService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _taskAssignmentService.GetAllTaskAssignments());
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> GetById(Guid Id)
    {
        var r = await _taskAssignmentService.GetTaskAssignemntById(Id);
        return Ok(r);
    }

    [HttpPost()]
    public async Task<IActionResult> Create(TaskAssignmentCreateDto taskAssignmentCreateDto)
    {
        ApiResponse<TaskAssignment> r = await _taskAssignmentService.CreateTaskAssignemnt(taskAssignmentCreateDto);

        return CreatedAtAction(
            nameof(GetById),
            new { Id = r.Data?.Id},
            r
        );
    }

    [HttpPut("{Id}/status")]
    public async Task<IActionResult> updateStatus(Guid Id, TaskAssignmentUpdateStatusDto taskAssignmentUpdateStatusDto)
    {
        var r = await _taskAssignmentService.UpdateTaskAssignemntStatus(Id, taskAssignmentUpdateStatusDto);
        return Ok(r);
    }
}