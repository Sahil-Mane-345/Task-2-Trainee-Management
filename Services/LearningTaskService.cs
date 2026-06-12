using Microsoft.EntityFrameworkCore;
using TraineeApi.Context;
using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.LearningTaskDTO;
using TraineeApi.Services.Interfaces;

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

        res.success = true;
        res.message = "LearningTasks fetched successfully.";
        res.data = learningTasks;

        return res;
    }

    public async Task<ApiResponse<LearningTask>> GetLearningTaskById(Guid Id)
    {
        Console.WriteLine("Wea ewsdsd 36" + Id);
        ApiResponse<LearningTask> res = new();

        LearningTask? learningTask = await _context.LearningTasks.FindAsync(Id);
        Console.WriteLine("Wea ewsdsd 39"+ learningTask?.Id);
        if(learningTask == null)
        {
            res.success = false;
            res.message = $"No LearningTask Found with Id : {Id}";
            _logger.LogError($"No LearningTask Found with Id : {Id}");
            return res;
        }
        res.success = true;
        res.message = $"LearningTask found with Id : {Id}";
        res.data = learningTask;
        return res;
    }

    public async Task<ApiResponse<LearningTask>> CreateLearningTask(LearningTaskCreateDto learningTaskCreateDto)
    {
        ApiResponse<LearningTask> res = new();

        LearningTask LearningTask = new()
        {
            Title = learningTaskCreateDto.Title,
            Description = learningTaskCreateDto.Description,
            ExpectedTechStack = learningTaskCreateDto.ExpectedTechStack,
            DueDate = learningTaskCreateDto.DueDate,
            Status = learningTaskCreateDto.Status
        };

        await _context.LearningTasks.AddAsync(LearningTask);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"LearningTask created with Id : {LearningTask.Id}");

        res.success = true;
        res.message = "LearningTask created successfully.";
        res.data = LearningTask;
        return res;
    }

    public async Task<ApiResponse<LearningTask>> UpdateLearningTask(Guid Id, LearningTaskUpdateDto learningTaskUpdateDto)
    {
        ApiResponse<LearningTask> res = new();

        LearningTask? learningTask = await _context.LearningTasks.FindAsync(Id);
        if( learningTask == null)
        {
            res.success = false;
            res.message = $"No LearningTask Found with Id : {Id}";
            _logger.LogError($"No LearningTask found with Id : {Id}");
            return res;
        }

        DateTime timestamp = DateTime.UtcNow;

        learningTask.Title = learningTaskUpdateDto.Title;
        learningTask.Description = learningTaskUpdateDto.Description;
        learningTask.ExpectedTechStack = learningTaskUpdateDto.ExpectedTechStack;
        learningTask.DueDate = learningTaskUpdateDto.DueDate;
        learningTask.Status = learningTaskUpdateDto.Status;
        
        learningTask.UpdatedDate = timestamp;

        await _context.SaveChangesAsync();

        _logger.LogInformation($"LearningTask data updated successfully for Id : {Id}");
        res.success = true;
        res.message = "LearningTask Updated Successfully.";
        res.data = learningTask;

        return res;
    }

    public async Task<bool> DeleteLearningTaskById(Guid Id)
    {
        LearningTask? LearningTask = await _context.LearningTasks.FindAsync(Id);

        if( LearningTask == null)
        {
            _logger.LogError($"No LearningTask found with Id : {Id}");
            return false;
        }
        _context.LearningTasks.Remove(LearningTask);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"LearningTask deletd succesfully with Id : {Id}");
        return true;
    }
}