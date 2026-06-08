using TraineeApi.Models.Entity;
using TraineeApi.Models;

namespace TraineeApi.Services.Interfaces{

public interface ITraineeService{
    Task<ApiResponse<List<Trainee>>> GetAllTrainee(string search);
    Task<ApiResponse<Trainee>> GetTraineeById(long Id);
    Task<ApiResponse<Trainee>> CreateTrainee(CreateTraineeRequest newTrainee);
    Task<ApiResponse<Trainee>> UpdateTrainee(long Id, UpdateTraineeRequest updateTrainee);
    Task<bool> DeleteTraineeById(long Id);
}

}