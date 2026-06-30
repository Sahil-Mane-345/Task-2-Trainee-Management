using Microsoft.EntityFrameworkCore;
using TraineeApi.Context;
using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.MentorDTo;
using TraineeApi.Services.Interfaces;
using TraineeApi.Services.Redis;
using TraineeApi.Utility.Exception;

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

        res.Success = true;
        res.Message = "Mentors fetched successfully.";
        res.Data = mentors;

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
                _logger.LogWarning("Mentor Not Found. MentorId : {Id}", Id);
                throw new NotFoundException($"Mentor Not found for this Id");
            }
            await _cache.SetAsync($"mentor:{Id}",mentor);
        }
        
        res.Success = true;
        res.Message = $"Mentor found with Id : {Id}";
        res.Data = mentor;
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

        _logger.LogInformation("Mentor created with Id : {mentor.Id}", mentor.Id);

        res.Success = true;
        res.Message = "Mentor created successfully.";
        res.Data = mentor;
        return res;
    }

    public async Task<ApiResponse<Mentor>> UpdateMentor(Guid Id, MentorUpdateDto mentorUpdateDto)
    {
        ApiResponse<Mentor> res = new();

        Mentor? mentor = await _context.Mentors.FindAsync(Id);
        if( mentor == null)
        {
            _logger.LogWarning("Mentor Not Found. MentorId : {Id}", Id);
            throw new NotFoundException($"Mentor Not found for this Id");
        }

        mentor.FirstName = mentorUpdateDto.FirstName;
        mentor.LastName = mentorUpdateDto.LastName;
        mentor.Email = mentorUpdateDto.Email;
        mentor.Expertise = mentorUpdateDto.Expertise;
        mentor.Status = mentorUpdateDto.Status;
        mentor.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _cache.RemoveAsync($"mentor:{Id}");

        _logger.LogInformation("Mentor data updated successfully for Id : {Id}",Id);
        res.Success = true;
        res.Message = "Mentor Updated Successfully.";
        res.Data = mentor;

        return res;
    }

    public async Task<bool> DeleteMentorById(Guid Id)
    {
        Mentor? mentor = await _context.Mentors.FindAsync(Id);

        if( mentor == null)
        {
            _logger.LogWarning("Mentor Not Found. MentorId : {Id}", Id);
            throw new NotFoundException($"Mentor Not found for this Id");
        }

        _context.Mentors.Remove(mentor);
        await _context.SaveChangesAsync();

        await _cache.RemoveAsync($"mentor:{Id}");
        _logger.LogInformation("Mentor deletd succesfully with Id : {Id}",Id);
        return true;
    }
}