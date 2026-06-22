using Microsoft.EntityFrameworkCore;
using TraineeApi.Context;
using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.MentorDTo;
using TraineeApi.Services.Interfaces;
using TraineeApi.Services.Redis;

namespace TraineeApi.Services;

public class MentorService : IMentorService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MentorService> _logger;
    private readonly IRedisService _cache;

    public MentorService(AppDbContext context, IRedisService cache, ILogger<MentorService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ApiResponse<List<Mentor>>> GetAllMentors()
    {
        ApiResponse<List<Mentor>> res = new();

        List<Mentor> mentors = await _context.Mentors.ToListAsync();

        res.success = true;
        res.message = "Mentors fetched successfully.";
        res.data = mentors;

        return res;
    }

    public async Task<ApiResponse<Mentor>> GetMentorById(Guid Id)
    {
        ApiResponse<Mentor> res = new();

        Mentor? mentor = await _cache.GetAsync<Mentor>($"mentor:{Id}");
        if(mentor == null)
        {
            mentor = await _context.Mentors.FirstOrDefaultAsync( m => m.Id == Id);
            if(mentor == null)
            {
                res.success = false;
                res.message = $"No Mentor Found with Id : {Id}";
                _logger.LogError($"No Mentor Found with Id : {Id}");
                return res;
            }
            await _cache.SetAsync($"mentor:{Id}",mentor);
        }
        
        res.success = true;
        res.message = $"Mentor found with Id : {Id}";
        res.data = mentor;
        return res;
    }

    public async Task<ApiResponse<Mentor>> CreateMentor(MentorCreateDto mentorCreateDto)
    {
        ApiResponse<Mentor> res = new();

        Mentor mentor = new()
        {
            FirstName = mentorCreateDto.FirstName,
            LastName = mentorCreateDto.LastName,
            Email = mentorCreateDto.Email,
            Expertise = mentorCreateDto.Expertise,
            Status = mentorCreateDto.Status
        };

        await _context.Mentors.AddAsync(mentor);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Mentor created with Id : {mentor.Id}");

        res.success = true;
        res.message = "Mentor created successfully.";
        res.data = mentor;
        return res;
    }

    public async Task<ApiResponse<Mentor>> UpdateMentor(Guid Id, MentorUpdateDto mentorUpdateDto)
    {
        ApiResponse<Mentor> res = new();

        Mentor? mentor = await _context.Mentors.FindAsync(Id);
        if( mentor == null)
        {
            res.success = false;
            res.message = $"No Mentor Found with Id : {Id}";
            _logger.LogError($"No Mentor found with Id : {Id}");
            return res;
        }

        DateTime timestamp = DateTime.UtcNow;

        mentor.FirstName = mentorUpdateDto.FirstName;
        mentor.LastName = mentorUpdateDto.LastName;
        mentor.Email = mentorUpdateDto.Email;
        mentor.Expertise = mentorUpdateDto.Expertise;
        mentor.Status = mentorUpdateDto.Status;
        mentor.UpdatedDate = timestamp;

        await _context.SaveChangesAsync();

        await _cache.RemoveAsync($"mentor:{Id}");

        _logger.LogInformation($"Mentor data updated successfully for Id : {Id}");
        res.success = true;
        res.message = "Mentor Updated Successfully.";
        res.data = mentor;

        return res;
    }

    public async Task<bool> DeleteMentorById(Guid Id)
    {
        Mentor? mentor = await _context.Mentors.FindAsync(Id);

        if( mentor == null)
        {
            _logger.LogError($"No Mentor found with Id : {Id}");
            return false;
        }
        _context.Mentors.Remove(mentor);
        await _context.SaveChangesAsync();

        await _cache.RemoveAsync($"mentor:{Id}");
        _logger.LogInformation($"Mentor deletd succesfully with Id : {Id}");
        return true;
    }
}