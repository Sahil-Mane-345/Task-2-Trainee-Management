using TraineeApi.Models.TraineeDTO;
using TraineeApi.Models.Entity;

using TraineeApi.Services.Interfaces;
using TraineeApi.Context;
using Microsoft.EntityFrameworkCore;
using TraineeApi.Models;

namespace TraineeApi.Services;

public class TraineeDbService : ITraineeService {

    private readonly ILogger<TraineeDbService> _logger;

    private readonly AppDbContext _context;

    public TraineeDbService(AppDbContext context, ILogger<TraineeDbService> logger){
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResponse<IEnumerable<Trainee>>>> GetAllTrainee(string search, int pageNumber, int pageSize, string status){
        ApiResponse<PagedResponse<IEnumerable<Trainee>>> res = new ApiResponse<PagedResponse<IEnumerable<Trainee>>>();
        var TData = await _context.Trainees.ToListAsync();

        
        // if(search != ""){
        //     var QuertT = TData.Where( t => t.FirstName.Contains(search) || t.LastName.Contains(search) || t.Email.Contains(search) || t.TechStack.Contains(search)).ToList();
        //     res.message = $"Trainee fetched for search : {search}";
        //     res.data = QuertT;
        // }
        
        IEnumerable<Trainee> QuerT = TData.Where( t => t.FirstName.Contains(search) || t.LastName.Contains(search) || t.Email.Contains(search) || t.TechStack.Contains(search)).Where( t => status.Equals("") || t.Status.Equals(status)).OrderBy( t => t.CreatedAt);

        int TotalCount = QuerT.Count();

        var Skip = (pageNumber - 1) * pageSize;
        IEnumerable<Trainee> PageR = QuerT.Skip(Skip).Take(pageSize);
        
        res.success = true;
        res.message = "Trainees fetched successfully.";
        
        PagedResponse<IEnumerable<Trainee>> pageRes = new PagedResponse<IEnumerable<Trainee>>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = TotalCount,
            Data = PageR
        };
        res.data = pageRes;
        return res;
    }

    public async Task<ApiResponse<Trainee>> GetTraineeById(Guid Id){
        ApiResponse<Trainee> res = new ApiResponse<Trainee>();
        var t = await _context.Trainees.FindAsync(Id);
        if ( t == null ){
            res.success = false;
            res.message = $"No Trainee Found with Id : {Id}";
            _logger.LogError($"No Trainee found with Id : {Id}");
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
            FirstName = newTrainee.FirstName!,
            LastName = newTrainee.LastName!,
            Email = newTrainee.Email!,
            TechStack = newTrainee.TechStack!,
            Status = newTrainee.Status!,
            CreatedAt = timestamp
            };

            await _context.Trainees.AddAsync(t);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Trainee created with Id : {t.Id}");

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
            _logger.LogError($"No Trainee found with Id : {Id}");
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
        _logger.LogInformation($"Trainee data updated successfully for Id : {Id}");
        res.success = true;
        res.message = "Trainee Updated Successfully.";
        res.data = t;

        return res;
    }

    public async Task<bool> DeleteTraineeById(Guid Id){
        var t = await _context.Trainees.FindAsync(Id);
        if( t == null){
            _logger.LogError($"No Trainee found with Id : {Id}");
            return false;
        }
        _context.Trainees.Remove(t);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Trainee deletd succesfully with Id : {Id}");
        return true;
    }
}