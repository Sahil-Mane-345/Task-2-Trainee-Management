using Microsoft.EntityFrameworkCore;
using TraineeApi.Context;
using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.TaskAssignmentDTO;
using TraineeApi.Services.Interfaces;

namespace TraineeApi.Services;

public class TaskAssignmentService : ITaskAssignmentService
{

    private readonly AppDbContext _context;

    private readonly ILogger<ITaskAssignmentService> _logger;

    public TaskAssignmentService(AppDbContext context, ILogger<TaskAssignmentService> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<ApiResponse<TaskAssignment>> CreateTaskAssignemnt(TaskAssignmentCreateDto taskAssignmentCreateDto)
    {
        ApiResponse<TaskAssignment> res = new();

        if(taskAssignmentCreateDto.AssignedDate > taskAssignmentCreateDto.DueDate)
        {
            throw new ArgumentException("Assigned Date should not be greater than Due Date");
        }

        bool mentor = await _context.Mentors.AnyAsync( m => m.Id == taskAssignmentCreateDto.MentorId);
        if( !mentor)
        {
            _logger.LogInformation($"No Mentor found with Id : {taskAssignmentCreateDto.MentorId}");
            throw new ArgumentException("Mentor with such Id does not exist");
        }

        bool trainee = await _context.Trainees.AnyAsync( t => t.Id == taskAssignmentCreateDto.TraineeId);
        if( !trainee)
        {
            _logger.LogInformation($"No Trainee found with Id : {taskAssignmentCreateDto.TraineeId}");
            throw new ArgumentException("Trainee with such Id does not exist");
        }

        bool learningTask = await _context.LearningTasks.AnyAsync( l => l.Id == taskAssignmentCreateDto.LearningTaskId);
        if( !learningTask)
        {
            _logger.LogInformation($"No Learning Task found with Id : {taskAssignmentCreateDto.LearningTaskId}");
            throw new ArgumentException("Learning Task with such Id does not exist");
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
        
        res.success = true;
        res.message = "Task Assignment Created Successfully";
        res.data = taskAssignment;

        return res;
    }

    public async Task<ApiResponse<List<TaskAssignment>>> GetAllTaskAssignments()
    {
        ApiResponse<List<TaskAssignment>> res = new();
        List<TaskAssignment> taskAssignments = await _context.TaskAssignments.ToListAsync();
        res.success = true;
        res.message = "All Task Assignments fetched successfully";
        res.data = taskAssignments;
        return res;
    }

    public async Task<ApiResponse<TaskAssignment>> GetTaskAssignemntById( Guid Id)
    {
        ApiResponse<TaskAssignment> res = new();
        TaskAssignment? taskAssignment = await _context.TaskAssignments.FindAsync(Id);
        if(taskAssignment == null)
        {
            res.success = false;
            res.message = $"No Task Assignment found with Id : {Id}";

            _logger.LogInformation($"No Task found with Id : {Id}");
            return res;
        }
        res.success = true;
        res.message = $"Task Assignment found with Id : {Id}";
        res.data = taskAssignment;
        return res;
    }

    public async Task<ApiResponse<TaskAssignment>> UpdateTaskAssignemntStatus(Guid Id, TaskAssignmentUpdateStatusDto taskAssignmentUpdateStatusDto)
    {
        ApiResponse<TaskAssignment> res = new();
        TaskAssignment? taskAssignment = await _context.TaskAssignments.FindAsync(Id);
        if(taskAssignment == null)
        {
            res.success = false;
            res.message = $"No Task Assignment Found with Id : {Id}";
            _logger.LogInformation($"No Task Assignment found with Id : {Id}");
            return res;
        }
        
        taskAssignment.Status = taskAssignmentUpdateStatusDto.Status!;
        taskAssignment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        res.success = true;
        res.message = $"Task Assignment Status Updated for Id : {Id}";
            
        return res;
    }
}