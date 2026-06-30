using Microsoft.EntityFrameworkCore;
using TraineeApi.Context;
using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.LearningTaskDTO;
using TraineeApi.Services.Interfaces;
using TraineeApi.Utility.Exception;

namespace TraineeApi.Services;

public class LearningTaskService : ILearningTaskService
{
    private readonly AppDbContext _context;
    private readonly ILogger<LearningTaskService> _logger;

    public LearningTaskService(AppDbContext context, ILogger<LearningTaskService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<List<LearningTask>>> GetAllLearningTasks()
    {
        ApiResponse<List<LearningTask>> res = new();

        List<LearningTask> learningTasks = await _context.LearningTasks.ToListAsync();

        res.Success = true;
        res.Message = "LearningTasks fetched successfully.";
        res.Data = learningTasks;

        return res;
    }

    public async Task<ApiResponse<LearningTask>> GetLearningTaskById(Guid Id)
    {

        ApiResponse<LearningTask> res = new();

        LearningTask? learningTask = await _context.LearningTasks.FindAsync(Id);
        if(learningTask == null)
        {
            _logger.LogWarning("Learning Task Not Found. LearningTaskId : {Id}", Id);
            throw new NotFoundException($"Learning Task Not found for this Id");
        }
        res.Success = true;
        res.Message = $"LearningTask found with Id : {Id}";
        res.Data = learningTask;
        return res;
    }

    public async Task<ApiResponse<LearningTask>> CreateLearningTask(LearningTaskCreateDto learningTaskCreateDto)
    {
        ApiResponse<LearningTask> res = new();

        LearningTask LearningTask = new()
        {
            Title = learningTaskCreateDto.Title!,
            Description = learningTaskCreateDto.Description!,
            ExpectedTechStack = learningTaskCreateDto.ExpectedTechStack!,
            DueDate = learningTaskCreateDto.DueDate,
            Status = learningTaskCreateDto.Status!
        };

        await _context.LearningTasks.AddAsync(LearningTask);
        await _context.SaveChangesAsync();

        _logger.LogInformation("LearningTask created with Id : {LearningTask.Id}",LearningTask.Id);

        res.Success = true;
        res.Message = "LearningTask created successfully.";
        res.Data = LearningTask;
        return res;
    }

    public async Task<ApiResponse<LearningTask>> UpdateLearningTask(Guid Id, LearningTaskUpdateDto learningTaskUpdateDto)
    {
        ApiResponse<LearningTask> res = new();

        LearningTask? learningTask = await _context.LearningTasks.FindAsync(Id);
        if( learningTask == null)
        {
            _logger.LogWarning("Learning Task Not Found. LearningTaskId : {Id}", Id);
            throw new NotFoundException($"Learning Task Not found for this Id");
        }

        learningTask.Title = learningTaskUpdateDto.Title;
        learningTask.Description = learningTaskUpdateDto.Description;
        learningTask.ExpectedTechStack = learningTaskUpdateDto.ExpectedTechStack;
        learningTask.DueDate = learningTaskUpdateDto.DueDate;
        learningTask.Status = learningTaskUpdateDto.Status;
        
        learningTask.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("LearningTask data updated successfully for Id : {Id}",Id);
        res.Success = true;
        res.Message = "LearningTask Updated Successfully.";
        res.Data = learningTask;

        return res;
    }

    public async Task<bool> DeleteLearningTaskById(Guid Id)
    {
        LearningTask? LearningTask = await _context.LearningTasks.FindAsync(Id);

        if( LearningTask == null)
        {
            _logger.LogWarning("Learning Task Not Found. LearningTaskId : {Id}", Id);
            throw new NotFoundException("No Learning Task found for this Id");
        }
        _context.LearningTasks.Remove(LearningTask);
        await _context.SaveChangesAsync();
        _logger.LogInformation("LearningTask deleted succesfully with Id : {Id}",Id);
        return true;
    }
}