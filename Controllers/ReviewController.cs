using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.ReviewDTO;
using TraineeApi.Services.Interfaces;

namespace TraineeApi.Controllers;


[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _reviewService.GetAllReviews());
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> GetById(Guid Id)
    {
        var r = await _reviewService.GetReviewById(Id);
        
        return Ok(r);
    }

    [HttpPost()]
    public async Task<IActionResult> Create(ReviewCreateDto reviewCreateDto)
    {
        ApiResponse<Review> r = await _reviewService.CreateReview(reviewCreateDto);

        return CreatedAtAction(
            nameof(GetById),
            new { Id = r.Data?.Id},
            r
        );
    }
}