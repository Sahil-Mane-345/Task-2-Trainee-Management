using System;
using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Services.Interfaces;

namespace TraineeApi.Services;



public class TraineeService: ITraineeService {
    private static List<Trainee> trainees = new List<Trainee>();

    public ApiResponse<List<Trainee>> GetAllTrainee(){
        ApiResponse<List<Trainee>> res = new ApiResponse<List<Trainee>>();
        try{
            res.Success = true;
            res.message = "Trainees fetched successfully.";
            res.Data = trainees;
            return res;
        }catch(Exception e){
            res.Success = false;
            res.message = e.ToString();
            res.Data = trainees;
            return res;
            
        }
    }

    public ApiResponse<Trainee> GetTraineeById(long id){
        ApiResponse<Trainee> res = new ApiResponse<Trainee>();
        try{
            var t = trainees.FirstOrDefault(t => (t.id == id));
            if( t == null ){
                res.Success = false;
                res.message = "No trainee found";
            }else{
                res.Success = true;
                res.message = "Trainee feched";
                res.Data = t;
            }
            return res;
        }catch(Exception e){
            res.Success = false;
            res.message = e.ToString();
            return res;
        }
    }

    public ApiResponse<Trainee> CreateTrainee(CreateTraineeRequest newTrainee){
        ApiResponse<Trainee> res = new ApiResponse<Trainee>();
        try{
            DateTime timestamp = DateTime.Now;

            long unixMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            Trainee t = new Trainee{
                id = unixMilliseconds,
                FirstName = newTrainee.FirstName,
                LastName = newTrainee.LastName,
                Email = newTrainee.Email,
                TechStack = newTrainee.TechStack,
                Status = newTrainee.Status,
                CreatedAt = timestamp
            };

            trainees.Add(t);
            res.Success = true;
            res.message = "Trainee created successfully.";
            res.Data = t;
            return res;

        }catch(Exception e){
            res.Success = false;
            res.message = e.ToString();
            return res;
        }
    }

    public ApiResponse<Trainee> UpdateTrainee(long id, UpdateTraineeRequest updateTrainee){
        ApiResponse<Trainee> res = new ApiResponse<Trainee>();
        try{
            var idx = trainees.FindIndex(t => t.id == id);
            if (idx != -1){
                DateTime timestamp = DateTime.Now;

                trainees[idx].FirstName = updateTrainee.FirstName;
                trainees[idx].LastName = updateTrainee.LastName;
                trainees[idx].Email = updateTrainee.Email;
                trainees[idx].TechStack = updateTrainee.TechStack;
                trainees[idx].Status = updateTrainee.Status;
                trainees[idx].UpdatedAt = timestamp;

                res.Success = true;
                res.message = "Trainee Edited Suuccessfully.";
                res.Data = trainees[idx];
                return res;
            }
            res.Success = false;
            res.message = "Trainee not found";
            return res;

        }catch(Exception e){
            res.Success = false;
            res.message = e.ToString();
            return res;
        }
    }

    public Boolean DeleteTraineeById(long id){
        
        try{
            var idx = trainees.FindIndex(t => t.id == id);
            if(idx != -1){
                trainees.RemoveAt(idx);
                return true;
            }
            return false;
        }catch(Exception e){
            Console.WriteLine(e.ToString());
            return false;
        }
    }
}