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

    private readonly IFileStorageService _fileStorageService;

    public SubmissionController(ISubmissionService submissionService, IFileStorageService fileStorageService)
    {
        _submissionService = submissionService;
        _fileStorageService = fileStorageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _submissionService.GetAllSubmissions());
    }

    [Authorize]
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

    [Authorize]
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

    [Authorize]
    [HttpPost("{submissionId}/files")]
    public async Task<IActionResult> SubmitFiles(Guid submissionId, SubmissionFilesDto submissionFilesDto)
    {
        var res = await _fileStorageService.SaveAsync(submissionId, submissionFilesDto.SubmissionFiles!);

        if (!res.success)
        {
            return Problem(statusCode:413,detail:res.message);
        }
        return Ok(res);
    }

    [Authorize]
    [HttpGet("/api/submission-files/{submissionFileId}/download")]
    public async Task<IActionResult> DownloadFiles(Guid submissionFileId)
    {
        var res = await _fileStorageService.OpenReadAsync(submissionFileId);

        if (!res.success)
        {
            return NotFound(res);
        }

        return File(res.data!.FileBytes, res.data.ContentType, fileDownloadName: res.data.DownloadString);
    }

    [Authorize]
    [HttpDelete("/api/submission-files/{submissionFileId}")]
    public async Task<IActionResult> DeleteFile(Guid submissionFileId)
    {
        bool r = await _fileStorageService.DeleteAsync(submissionFileId);
        if( !r)
        {
            return NotFound( new { message = $"File with Id Not found "});
        }
        return NoContent();
    }
}