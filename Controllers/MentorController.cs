using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeApi.Models.MentorDTo;
using TraineeApi.Services.Interfaces;

namespace TraineeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class MentorController : ControllerBase
{
    private readonly IMentorService _mentorService;

    public MentorController(IMentorService mentorService)
    {
        _mentorService = mentorService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _mentorService.GetAllMentors());
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> GetById(Guid Id)
    {
        var r = await _mentorService.GetMentorById(Id);
        return Ok(r);
    }

    [HttpPost()]
    public async Task<IActionResult> Create(MentorCreateDto mentorCreateDto)
    {
        var r = await _mentorService.CreateMentor(mentorCreateDto);
        return CreatedAtAction(
            nameof(GetById),
            new { Id = r?.Data?.Id},
            r
        );
    }
    

    [HttpPut("{Id}")]
    public async Task<IActionResult> UpdateById(Guid Id, MentorUpdateDto mentorUpdateDto)
    {
        var r = await _mentorService.UpdateMentor(Id, mentorUpdateDto);
        return Ok(r);
        
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeleteById(Guid Id){
        bool t = await _mentorService.DeleteMentorById(Id);
        return NoContent();
    }
    
}