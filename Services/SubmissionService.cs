using Microsoft.EntityFrameworkCore;
using TraineeApi.Context;
using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.SubmissionDTO;
using TraineeApi.Services.Interfaces;
using TraineeApi.Services.Redis;
using TraineeApi.Utility.Exception;

namespace TraineeApi.Services;

public class SubmissionService : ISubmissionService
{
    private readonly AppDbContext _context;
    private readonly IRedisService _cache;
    private readonly ILogger<SubmissionService> _logger;

    public SubmissionService(AppDbContext context, ILogger<SubmissionService> logger, IRedisService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<ApiResponse<Submission>> CreateSubmission(SubmissionCreateDto submissionCreateDto)
    {
        ApiResponse<Submission> res = new();

        bool TaskAssignment = await _context.TaskAssignments.AnyAsync( t => t.Id == submissionCreateDto.TaskAssignmentId);

        if (!TaskAssignment)
        {
            _logger.LogInformation($"No Task Assignment found with Id : {submissionCreateDto.TaskAssignmentId}");
            throw new InvalidIdentifierException("Task Assignment with such Id does not exist");
        }

        Submission? existingSubmission = await _context.Submissions.FirstOrDefaultAsync( s => s.TaskAssignmentId == submissionCreateDto.TaskAssignmentId);

        if( existingSubmission != null)
        {
            existingSubmission.SubmissionUrl = submissionCreateDto.SubmissionUrl!;
            existingSubmission.Notes = submissionCreateDto.Notes;
            existingSubmission.SubmittedDate = DateOnly.FromDateTime(DateTime.UtcNow);
            existingSubmission.Status = "Resubmitted";
            existingSubmission.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _cache.RemoveAsync($"submission:{existingSubmission.Id}");
            res.Success = true;
            res.Message = $"Submission with Id : {existingSubmission?.Id} resubmitted successfully";
            res.Data = existingSubmission;

            return res;
        }

        Submission submission = new()
        {
            TaskAssignmentId = (Guid)submissionCreateDto.TaskAssignmentId!,
            SubmissionUrl = submissionCreateDto.SubmissionUrl!,
            Notes = submissionCreateDto.Notes,
            SubmittedDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Status = "Submitted",
        };
        TaskAssignment? taskAssignment = await _context.TaskAssignments.FindAsync(submission.TaskAssignmentId);

        if(taskAssignment == null)
        {
            _logger.LogInformation($"No Task Assignment found with Id : {submissionCreateDto.TaskAssignmentId}");
            throw new InvalidIdentifierException("Task Assignment with such Id does not exist");
        }

        taskAssignment.Status = "Submitted";
        taskAssignment.UpdatedAt = DateTime.UtcNow;

        await _context.Submissions.AddAsync(submission);
        await _context.SaveChangesAsync();

        submission.TaskAssignment = null!;

        res.Success = true;
        res.Message = $"Submission submitted successfully";
        res.Data = submission;
        return res;


    }

    public async Task<ApiResponse<List<Submission>>> GetAllSubmissions()
    {
        ApiResponse<List<Submission>> res = new();
        
        List<Submission> submissions = await _context.Submissions.ToListAsync();

        res.Success = true;
        res.Message = $"Submissions fetched successfully";
        res.Data = submissions;
        return res;
    }

    public async Task<ApiResponse<Submission>> GetSubmissionById(Guid Id)
    {
        ApiResponse<Submission> res = new();
        
        Submission? submission = await _cache.GetAsync<Submission>($"submission:{Id}");
        if(submission == null)
        {
            submission = await _context.Submissions.FindAsync(Id);

            if( submission == null)
            {
               _logger.LogInformation($"No Submission found with Id : {Id}");
            throw new NotFoundException("No Submission found for this Id"); 
            }

            await _cache.SetAsync($"submission:{Id}",submission);
        }
        

        res.Success = true;
        res.Message = $"Submission Found with Id : {Id}";
        res.Data = submission;
        
        return res;
    }
}