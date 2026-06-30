using Microsoft.EntityFrameworkCore;
using TraineeApi.Context;
using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.ReviewDTO;
using TraineeApi.Services.Interfaces;
using TraineeApi.Utility.Exception;

namespace TraineeApi.Services;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _context;

    private readonly ILogger<ReviewService> _logger;

    public ReviewService(AppDbContext context, ILogger<ReviewService> logger)
    {
        _context = context;
        _logger = logger;   
    }

    public async Task<ApiResponse<Review>> CreateReview(ReviewCreateDto reviewCreateDto)
    {
        ApiResponse<Review> res = new();

       Submission? submission = await _context.Submissions.FirstOrDefaultAsync( t => t.Id == reviewCreateDto.SubmissionId);

        if (submission == null)
        {
            _logger.LogWarning("No Submission found with Id : {reviewCreateDto.SubmissionId}",reviewCreateDto.SubmissionId);
            throw new InvalidIdentifierException("Submission with such Id does not exist");
        }

         bool mentor = await _context.Mentors.AnyAsync( t => t.Id == reviewCreateDto.MentorId);

        if (!mentor)
        {
            _logger.LogWarning("No Mentor found with Id : {reviewCreateDto.MentorId}",reviewCreateDto.MentorId);
            throw new InvalidIdentifierException("Mentor with such Id does not exist");
        }

        Review? existingReview = await _context.Reviews.FirstOrDefaultAsync( r => r.SubmissionId == reviewCreateDto.SubmissionId);

        TaskAssignment? taskAssignment = await _context.TaskAssignments.FindAsync(submission.TaskAssignmentId);

        if( existingReview != null)
        {
            existingReview.Feedback = reviewCreateDto.Feedback;
            existingReview.Score = reviewCreateDto.Score;
            existingReview.ReviewStatus = reviewCreateDto.ReviewStatus!;
            existingReview.ReviewdDate = DateOnly.FromDateTime(DateTime.UtcNow);
            existingReview.UpdatedAt = DateTime.UtcNow;

            if(taskAssignment == null)
            {
                _logger.LogWarning("No Task found with Id : {submission.TaskAssignmentId}",submission.TaskAssignmentId);
                throw new NotFoundException("No Task Assigned found for this Id");
            }

            taskAssignment.Status = reviewCreateDto.ReviewStatus == "Accepted" ? "Completed" : "Reviewed";
            taskAssignment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            existingReview.Submission = null!;
            res.Success = true;
            res.Message = "Review Submitted successfully";
            res.Data = existingReview;
            return res;
        }

        Review review = new()
        {
            SubmissionId = (Guid)reviewCreateDto.SubmissionId!,
            MentorId = (Guid)reviewCreateDto.MentorId!,
            Feedback = reviewCreateDto.Feedback,
            Score = reviewCreateDto.Score,
            ReviewStatus = reviewCreateDto.ReviewStatus,
            ReviewdDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };


        if(taskAssignment == null)
        {
            _logger.LogWarning("No Task found with Id : {submission.TaskAssignmentId}",submission.TaskAssignmentId);
            throw new NotFoundException("No Task Assigned found for this Id");
        }

        taskAssignment.Status = reviewCreateDto.ReviewStatus == "Accepted" ? "Completed" : "Reviewed";
        taskAssignment.UpdatedAt = DateTime.UtcNow;

        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Review Submitted successfully. Id: {review.Id}", review.Id);

        review.Submission = null!;
        res.Success = true;
        res.Message = $"Review submitted successfully";
        res.Data = review;
        return res;

    }

    public async Task<ApiResponse<List<Review>>> GetAllReviews()
    {
        ApiResponse<List<Review>> res = new();

        List<Review> reviews = await _context.Reviews.ToListAsync();

        res.Success = true;
        res.Message = $"Reviews fetched successfully";
        res.Data = reviews;
        return res;
    }

    public async Task<ApiResponse<Review>> GetReviewById(Guid Id)
    {
        ApiResponse<Review> res = new();
        
        Review? review = await _context.Reviews.FindAsync(Id);

        if( review == null)
        {
            _logger.LogWarning("No Review found with Id : {Id}", Id);
            throw new NotFoundException("No Review found for this Id");
        }

        res.Success = true;
        res.Message = $"Submission Found with Id : {Id}";
        res.Data = review;
        
        return res;
    }
}