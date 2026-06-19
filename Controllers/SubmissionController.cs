using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.SubmissionDTO;
using TraineeApi.Services.Interfaces;

namespace TraineeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class SubmissionController : ControllerBase
{
    private readonly ISubmissionService _submissionService;

    public SubmissionController(ISubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _submissionService.GetAllSubmissions());
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> GetById(Guid Id)
    {
        var r = await _submissionService.GetSubmissionById(Id);
        if (!r.success)
        {
            return NotFound(r);
        }
        return Ok(r);
    }

    [HttpPost()]
    public async Task<IActionResult> Create(SubmissionCreateDto submissionCreateDto)
    {
        ApiResponse<Submission> r = await _submissionService.CreateSubmission(submissionCreateDto);

        return CreatedAtAction(
            nameof(GetById),
            new { Id = r.data?.Id},
            r
        );
    }
}