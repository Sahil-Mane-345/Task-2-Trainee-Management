using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeApi.MessageBroker.Entity;
using TraineeApi.MessageBroker.Services;
using TraineeApi.Services.Interfaces;

namespace TraineeApi.Controllers;

[ApiController]
[Route("[controller]")]
// [Authorize]
public class ProcessingJobsController: ControllerBase
{
    private readonly IProcessingJobService _processingJobService;

    public ProcessingJobsController(IProcessingJobService processingJobService)
    {
        _processingJobService = processingJobService;
    }

    [HttpGet("/api/processing-jobs")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _processingJobService.GetAll());
    }

    [HttpGet("/api/processing-jobs/{Id}")]
    public async Task<IActionResult> GetStatusById(Guid Id)
    {
        return Ok(await _processingJobService.GetStatusById(Id));
    }

    [HttpPost("/api/processing-jobs/{Id}/retry")]
    public async Task<IActionResult> RetryJob(Guid Id)
    {
        return Ok(await _processingJobService.RetryJob(Id));
    }
}