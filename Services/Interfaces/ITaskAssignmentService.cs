using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.TaskAssignmentDTO;

namespace TraineeApi.Services.Interfaces;

public interface ITaskAssignmentService
{
    Task<ApiResponse<List<TaskAssignment>>> GetAllTaskAssignments();

    Task<ApiResponse<TaskAssignment>> GetTaskAssignemntById(Guid Id);

    Task<ApiResponse<TaskAssignment>> CreateTaskAssignemnt(TaskAssignmentCreateDto taskAssignmentCreateDto);

    Task<ApiResponse<TaskAssignment>> UpdateTaskAssignemntStatus(Guid Id, TaskAssignmentUpdateStatusDto taskAssignmentUpdateStatusDto);
}