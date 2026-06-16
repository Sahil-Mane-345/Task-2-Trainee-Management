using Microsoft.EntityFrameworkCore;
using TraineeApi.Context;
using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.SubmissionDTO;
using TraineeApi.Services.Interfaces;

namespace TraineeApi.Services;

public class SubmissionService : ISubmissionService
{
    private readonly AppDbContext _context;

    private readonly ILogger<SubmissionService> _logger;

    public SubmissionService(AppDbContext context, ILogger<SubmissionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<Submission>> CreateSubmission(SubmissionCreateDto submissionCreateDto)
    {
        ApiResponse<Submission> res = new();

        bool TaskAssignment = await _context.TaskAssignments.AnyAsync( t => t.Id == submissionCreateDto.TaskAssignmentId);

        if (!TaskAssignment)
        {
            _logger.LogInformation($"No Task Assignment found with Id : {submissionCreateDto.TaskAssignmentId}");
            throw new ArgumentException("Task Assignment with such Id does not exist");
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

            res.success = true;
            res.message = $"Submission with Id : {existingSubmission?.Id} resubmitted successfully";
            res.data = existingSubmission;

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

        if(taskAssignment != null)
        {
            taskAssignment.Status = "Submitted";
            taskAssignment.UpdatedAt = DateTime.UtcNow;
        }
        await _context.Submissions.AddAsync(submission);
        await _context.SaveChangesAsync();

        submission.TaskAssignment = null!;

        res.success = true;
        res.message = $"Submission submitted successfully";
        res.data = submission;
        return res;


    }

    public async Task<ApiResponse<List<Submission>>> GetAllSubmissions()
    {
        ApiResponse<List<Submission>> res = new();
        
        List<Submission> submissions = await _context.Submissions.ToListAsync();

        res.success = true;
        res.message = $"Submissions fetched successfully";
        res.data = submissions;
        return res;
    }

    public async Task<ApiResponse<Submission>> GetSubmissionById(Guid Id)
    {
        ApiResponse<Submission> res = new();
        
        Submission? submission = await _context.Submissions.FindAsync(Id);

        if( submission == null)
        {
            res.success = false;
            res.message = $"No Submission Found with Id : {Id}";
            _logger.LogError($"No Submission found with Id : {Id}");
            return res;
        }

        res.success = true;
        res.message = $"Submission Found with Id : {Id}";
        res.data = submission;
        
        return res;
    }
}