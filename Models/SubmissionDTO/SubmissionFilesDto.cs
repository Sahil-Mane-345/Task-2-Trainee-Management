
using System.ComponentModel.DataAnnotations;

namespace TraineeApi.Models.SubmissionDTO;

public class SubmissionFilesDto
{
    [Required(ErrorMessage = "Submission Files are required")]
    public IFormFileCollection? SubmissionFiles { get; set; }
}