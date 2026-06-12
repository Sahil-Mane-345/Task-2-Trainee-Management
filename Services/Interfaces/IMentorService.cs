using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.MentorDTo;


namespace TraineeApi.Services.Interfaces;

public interface IMentorService
{
    Task<ApiResponse<List<Mentor>>> GetAllMentors();

    Task<ApiResponse<Mentor>> GetMentorById(Guid Id);

    Task<ApiResponse<Mentor>> CreateMentor(MentorCreateDto mentorCreateDto);

    Task<ApiResponse<Mentor>> UpdateMentor(Guid Id, MentorUpdateDto mentorUpdateDto);

    Task<bool> DeleteMentorById(Guid Id);
}