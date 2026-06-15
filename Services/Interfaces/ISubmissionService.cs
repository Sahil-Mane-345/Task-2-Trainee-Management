using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.SubmissionDTO;

namespace TraineeApi.Services.Interfaces;

public interface ISubmissionService
{
    Task<ApiResponse<List<Submission>>> GetAllSubmissions();

    Task<ApiResponse<Submission>> GetSubmissionById(Guid Id);

    Task<ApiResponse<Submission>> CreateSubmission(SubmissionCreateDto submissionCreateDto);
}