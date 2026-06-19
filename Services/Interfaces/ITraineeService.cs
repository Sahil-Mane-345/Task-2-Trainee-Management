using TraineeApi.Models.Entity;
using TraineeApi.Models;
using TraineeApi.Models.TraineeDTO;

namespace TraineeApi.Services.Interfaces{

public interface ITraineeService{
    ApiResponse<PagedResponse<IQueryable<Trainee>>> GetAllTrainee(string search, int pageNumber, int pageSize, string status);
    Task<ApiResponse<Trainee>> GetTraineeById(Guid Id);
    Task<ApiResponse<Trainee>> CreateTrainee(CreateTraineeRequest newTrainee);
    Task<ApiResponse<Trainee>> UpdateTrainee(Guid Id, UpdateTraineeRequest updateTrainee);
    Task<bool> DeleteTraineeById(Guid Id);
        
    }

}