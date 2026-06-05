using TraineeApi.Models.Entity;
using TraineeApi.Models;

namespace TraineeApi.Services.Interfaces{

public interface ITraineeService{
    ApiResponse<List<Trainee>> GetAllTrainee();
    ApiResponse<Trainee> GetTraineeById(long id);
    ApiResponse<Trainee> CreateTrainee(CreateTraineeRequest newTrainee);
    ApiResponse<Trainee> UpdateTrainee(long id, UpdateTraineeRequest updateTrainee);
    bool DeleteTraineeById(long id);
}

}