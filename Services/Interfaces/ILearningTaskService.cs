using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.LearningTaskDTO;

namespace TraineeApi.Services.Interfaces;

public interface ILearningTaskService
{
    Task<ApiResponse<List<LearningTask>>> GetAllLearningTasks();

    Task<ApiResponse<LearningTask>> GetLearningTaskById(Guid Id);

    Task<ApiResponse<LearningTask>> CreateLearningTask(LearningTaskCreateDto learningTaskCreateDto);

    Task<ApiResponse<LearningTask>> UpdateLearningTask(Guid Id, LearningTaskUpdateDto learningTaskUpdateDto);

    Task<bool> DeleteLearningTaskById(Guid Id);
}