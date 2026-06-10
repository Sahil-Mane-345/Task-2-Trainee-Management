using TraineeApi.Models.TraineeDTO;
using TraineeApi.Models.Entity;

using TraineeApi.Services.Interfaces;
using TraineeApi.Context;
using Microsoft.EntityFrameworkCore;
using TraineeApi.Models;

namespace TraineeApi.Services;

public class TraineeDbService : ITraineeService {

    private readonly AppDbContext _context;

    public TraineeDbService(AppDbContext context){
        _context = context;
    }

    public async Task<ApiResponse<List<Trainee>>> GetAllTrainee(string search){
        ApiResponse<List<Trainee>> res = new ApiResponse<List<Trainee>>();
        var TData = await _context.Trainees.ToListAsync();

        res.success = true;
        res.message = "Trainees fetched successfully.";
        
        res.data = TData;
        
        if(search != ""){
            var QuertT = TData.Where( t => t.FirstName.Contains(search) || t.LastName.Contains(search) || t.Email.Contains(search) || t.TechStack.Contains(search)).ToList();
            res.message = $"Trainee fetched for search : {search}";
            res.data = QuertT;
        }
        


        return res;
    }

    public async Task<ApiResponse<Trainee>> GetTraineeById(Guid Id){
        ApiResponse<Trainee> res = new ApiResponse<Trainee>();
        var t = await _context.Trainees.FindAsync(Id);
        if ( t == null ){
            res.success = false;
            res.message = $"No Trainee Found with Id : {Id}";

            return res;
        }
        res.success = true;
        res.message = $"Trainee found with Id : {Id}";
        res.data = t;
        return res;
    }

    public async Task<ApiResponse<Trainee>> CreateTrainee(CreateTraineeRequest newTrainee){
        ApiResponse<Trainee> res = new ApiResponse<Trainee>();
        DateTime timestamp = DateTime.Now;

        Trainee t = new Trainee{
            FirstName = newTrainee.FirstName,
            LastName = newTrainee.LastName,
            Email = newTrainee.Email,
            TechStack = newTrainee.TechStack,
            Status = newTrainee.Status,
            CreatedAt = timestamp
            };

            await _context.Trainees.AddAsync(t);
            await _context.SaveChangesAsync();

            res.success = true;
            res.message = "Trainee created successfully.";
            res.data = t;
            return res;
    }

    public async Task<ApiResponse<Trainee>> UpdateTrainee(Guid Id, UpdateTraineeRequest updateTrainee){
        ApiResponse<Trainee> res = new ApiResponse<Trainee>();

        var t = await _context.Trainees.FindAsync(Id);
        if ( t == null ){
            res.success = false;
            res.message = $"No Trainee Found with Id : {Id}";

            return res;
        }

        DateTime timestamp = DateTime.Now;
        t.FirstName = updateTrainee.FirstName;
        t.LastName = updateTrainee.LastName;
        t.Email = updateTrainee.Email;
        t.TechStack = updateTrainee.TechStack;
        t.Status = updateTrainee.Status;
        t.UpdatedAt = timestamp;

        await _context.SaveChangesAsync();

        res.success = true;
        res.message = "Trainee Updated Successfully.";
        res.data = t;

        return res;
    }

    public async Task<bool> DeleteTraineeById(Guid Id){
        var t = await _context.Trainees.FindAsync(Id);
        if( t == null){
            return false;
        }
        _context.Trainees.Remove(t);
        await _context.SaveChangesAsync();
        return true;
    }
}