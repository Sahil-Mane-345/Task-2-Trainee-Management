using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.ReviewDTO;

namespace TraineeApi.Services.Interfaces;

public interface IReviewService
{
    Task<ApiResponse<List<Review>>> GetAllReviews();

    Task<ApiResponse<Review>> GetReviewById(Guid Id);

    Task<ApiResponse<Review>> CreateReview(ReviewCreateDto reviewCreateDto);
}