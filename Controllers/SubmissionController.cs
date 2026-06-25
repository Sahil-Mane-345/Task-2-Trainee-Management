using System.Security.Claims;
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

    [HttpGet("{Id}")]
    public async Task<IActionResult> GetById(Guid Id)
    {
        var r = await _submissionService.GetSubmissionById(Id);
        return Ok(r);
    }

    [HttpPost()]
    public async Task<IActionResult> Create(SubmissionCreateDto submissionCreateDto)
    {
        ApiResponse<Submission> r = await _submissionService.CreateSubmission(submissionCreateDto);

        return CreatedAtAction(
            nameof(GetById),
            new { Id = r.Data?.Id},
            r
        );
    }

    [HttpPost("{submissionId}/files")]
    public async Task<IActionResult> SubmitFiles(Guid submissionId, SubmissionFilesDto submissionFilesDto)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var res = await _fileStorageService.SaveAsync(submissionId, new Guid(userId!) ,submissionFilesDto.SubmissionFiles!);
        return Ok(res);
    }

    [HttpGet("/api/submission-files/{submissionFileId}/download")]
    public async Task<IActionResult> DownloadFiles(Guid submissionFileId)
    {
        var res = await _fileStorageService.OpenReadAsync(submissionFileId);

        return File(res.Data!.FileBytes, res.Data.ContentType, fileDownloadName: res.Data.DownloadString);
    }

    [HttpDelete("/api/submission-files/{submissionFileId}")]
    public async Task<IActionResult> DeleteFile(Guid submissionFileId)
    {
        bool r = await _fileStorageService.DeleteAsync(submissionFileId);
        return NoContent();
    }
}