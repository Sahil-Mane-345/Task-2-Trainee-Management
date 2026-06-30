using Microsoft.EntityFrameworkCore;
using TraineeApi.Context;
using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.TaskAssignmentDTO;
using TraineeApi.Services.Interfaces;
using TraineeApi.Services.Redis;
using TraineeApi.Utility.Exception;

namespace TraineeApi.Services;

public class TaskAssignmentService : ITaskAssignmentService
{

    private readonly AppDbContext _context;

    private readonly ILogger<ITaskAssignmentService> _logger;

    private readonly IRedisService _cache;

    public TaskAssignmentService(AppDbContext context, IRedisService cache ,ILogger<TaskAssignmentService> logger)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }
    public async Task<ApiResponse<TaskAssignment>> CreateTaskAssignemnt(TaskAssignmentCreateDto taskAssignmentCreateDto)
    {
        ApiResponse<TaskAssignment> res = new();

        if(taskAssignmentCreateDto.AssignedDate > taskAssignmentCreateDto.DueDate)
        {
            throw new InvalidValidationException("Assigned Date should not be greater than Due Date");
        }

        bool mentor = await _context.Mentors.AnyAsync( m => m.Id == taskAssignmentCreateDto.MentorId);
        if( !mentor)
        {
            _logger.LogWarning("No Mentor found with Id : {taskAssignmentCreateDto.MentorId}", taskAssignmentCreateDto.MentorId);
            throw new InvalidIdentifierException("Mentor with such Id does not exist");
        }

        bool trainee = await _context.Trainees.AnyAsync( t => t.Id == taskAssignmentCreateDto.TraineeId);
        if( !trainee)
        {
            _logger.LogWarning("No Trainee found with Id : {taskAssignmentCreateDto.TraineeId}",taskAssignmentCreateDto.TraineeId);
            throw new InvalidIdentifierException("Trainee with such Id does not exist");
        }

        bool learningTask = await _context.LearningTasks.AnyAsync( l => l.Id == taskAssignmentCreateDto.LearningTaskId);
        if( !learningTask)
        {
            _logger.LogWarning("No Learning Task found with Id : {taskAssignmentCreateDto.LearningTaskId}",taskAssignmentCreateDto.LearningTaskId);
            throw new InvalidIdentifierException("Learning Task with such Id does not exist");
        }

        TaskAssignment taskAssignment = new()
        {
            TraineeId = (Guid)taskAssignmentCreateDto.TraineeId!,
            MentorId = (Guid)taskAssignmentCreateDto.MentorId!,
            LearningTaskId = (Guid)taskAssignmentCreateDto.LearningTaskId!,
            AssignedDate = taskAssignmentCreateDto.AssignedDate,
            DueDate = taskAssignmentCreateDto.DueDate,
            Status = "Assigned",
            Remarks = taskAssignmentCreateDto.Remarks!,
        };

        await _context.TaskAssignments.AddAsync(taskAssignment);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Task Assignment Created Successfully. TaskAssignmentId: {Id}", taskAssignment.Id);
        
        res.Success = true;
        res.Message = "Task Assignment Created Successfully";
        res.Data = taskAssignment;


        return res;
    }

    public async Task<ApiResponse<List<TaskAssignment>>> GetAllTaskAssignments()
    {
        ApiResponse<List<TaskAssignment>> res = new();
        List<TaskAssignment> taskAssignments = await _context.TaskAssignments.ToListAsync();
        res.Success = true;
        res.Message = "All Task Assignments fetched successfully";
        res.Data = taskAssignments;
        return res;
    }

    public async Task<ApiResponse<TaskAssignment>> GetTaskAssignemntById( Guid Id)
    {
        ApiResponse<TaskAssignment> res = new();
        TaskAssignment? taskAssignment = await _cache.GetAsync<TaskAssignment>($"taskassignment:{Id}");

        if( taskAssignment == null)
        {
            taskAssignment = await _context.TaskAssignments.FindAsync(Id);
            if(taskAssignment == null)
            {
                _logger.LogWarning("No Task Assignment found with Id : {Id}", Id);
                throw new NotFoundException("No Task Assigned found for this Id");
            }
            await _cache.SetAsync($"taskassignment:{Id}",taskAssignment);
        }
        
        res.Success = true;
        res.Message = $"Task Assignment found with Id : {Id}";
        res.Data = taskAssignment;
        return res;
    }

    public async Task<ApiResponse<TaskAssignment>> UpdateTaskAssignemntStatus(Guid Id, TaskAssignmentUpdateStatusDto taskAssignmentUpdateStatusDto)
    {
        ApiResponse<TaskAssignment> res = new();
        TaskAssignment? taskAssignment = await _context.TaskAssignments.FindAsync(Id);
        if(taskAssignment == null)
        {
            _logger.LogWarning("No Task found with Id : {Id}",Id);
            throw new NotFoundException("No Task Assigned found for this Id");
        }
        
        taskAssignment.Status = taskAssignmentUpdateStatusDto.Status!;
        taskAssignment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await _cache.RemoveAsync($"taskassignment:{Id}");
        _logger.LogInformation("Task Assignment deleted successfully. Id: {Id}", Id);

        res.Success = true;
        res.Message = $"Task Assignment Status Updated for Id : {Id}";
            
        return res;
    }
}