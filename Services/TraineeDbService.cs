using TraineeApi.Models.TraineeDTO;
using TraineeApi.Models.Entity;
using TraineeApi.Services.Interfaces;
using TraineeApi.Context;
using TraineeApi.Models;
using TraineeApi.Services.Redis;
using TraineeApi.Utility.Exception;

namespace TraineeApi.Services;

public class TraineeDbService : ITraineeService {

    private readonly ILogger<TraineeDbService> _logger;

    private readonly AppDbContext _context;

    private readonly IRedisService _cache;

    public TraineeDbService(AppDbContext context, IRedisService cache ,ILogger<TraineeDbService> logger){
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public ApiResponse<PagedResponse<IQueryable<Trainee>>> GetAllTrainee(string search, int pageNumber, int pageSize, string status){
        ApiResponse<PagedResponse<IQueryable<Trainee>>> res = new();
        
        IQueryable<Trainee> QuerT = _context.Trainees.Where( t => t.FirstName.Contains(search) || t.LastName.Contains(search) || t.Email.Contains(search) || t.TechStack.Contains(search)).Where( t => status.Equals("") || t.Status.Equals(status)).OrderBy( t => t.CreatedAt);

        int TotalCount = QuerT.Count();
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 || pageSize > 20 ? 10 : pageSize;
        var Skip = (pageNumber - 1) * pageSize;
        IQueryable<Trainee> PageR = QuerT.Skip(Skip).Take(pageSize);
        
        res.Success = true;
        res.Message = "Trainees fetched successfully.";
        
        PagedResponse<IQueryable<Trainee>> pageRes = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = TotalCount,
            Data = PageR
        };
        res.Data = pageRes;
        return res;
    }

    public async Task<ApiResponse<Trainee>> GetTraineeById(Guid Id){
        ApiResponse<Trainee> res =new();

        Trainee? trainee = await _cache.GetAsync<Trainee>($"trainee:{Id}");

        if( trainee == null)
        {
            var t = await _context.Trainees.FindAsync(Id);
            if( t == null ){
                _logger.LogWarning("Trainee Not Found. TraineeId : {Id}", Id);
                throw new NotFoundException($"Trainee Not found for this Id");
            }

            await _cache.SetAsync($"trainee:{Id}",t);
            res.Success = true;
            res.Message = $"Trainee found with Id from DB : {Id}";
            res.Data = t;
            return res;
        }
        res.Success = true;
        res.Message = $"Trainee found with Id  from cache: {Id}";
        res.Data = trainee;
        return res;
    }

    public async Task<ApiResponse<Trainee>> CreateTrainee(CreateTraineeRequest newTrainee){
        ApiResponse<Trainee> res = new ApiResponse<Trainee>();

        Trainee t = new Trainee{
            FirstName = newTrainee.FirstName!,
            LastName = newTrainee.LastName!,
            Email = newTrainee.Email!,
            TechStack = newTrainee.TechStack!,
            Status = newTrainee.Status!,
            CreatedAt = DateTime.UtcNow
            };

            await _context.Trainees.AddAsync(t);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Trainee created with Id : {t.Id}", t.Id);

            res.Success = true;
            res.Message = "Trainee created successfully.";
            res.Data = t;
            return res;
    }

    public async Task<ApiResponse<Trainee>> UpdateTrainee(Guid Id, UpdateTraineeRequest updateTrainee){
        ApiResponse<Trainee> res = new ApiResponse<Trainee>();

        var t = await _context.Trainees.FindAsync(Id);
        if ( t == null ){
            _logger.LogWarning("Trainee Not Found. TraineeId : {Id}", Id);
            throw new NotFoundException($"Trainee Not found for this Id");
        }

        t.FirstName = updateTrainee.FirstName;
        t.LastName = updateTrainee.LastName;
        t.Email = updateTrainee.Email;
        t.TechStack = updateTrainee.TechStack;
        t.Status = updateTrainee.Status;
        t.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Trainee data updated successfully for Id : {Id}", Id);

        await _cache.RemoveAsync($"trainee:{Id}");

        res.Success = true;
        res.Message = "Trainee Updated Successfully.";
        res.Data = t;

        return res;
    }

    public async Task<bool> DeleteTraineeById(Guid Id){
        var t = await _context.Trainees.FindAsync(Id);

        if( t == null){
            _logger.LogWarning("Trainee Not Found. TraineeId : {Id}", Id);
            throw new NotFoundException($"Trainee Not found for this Id");
        }
        _context.Trainees.Remove(t);
        await _context.SaveChangesAsync();

        await _cache.RemoveAsync($"trainee:{Id}");
        _logger.LogInformation("Trainee deletd succesfully. TraineeId : {Id}", Id);
        return true;
    }
}