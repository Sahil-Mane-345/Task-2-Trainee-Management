namespace TraineeApi.Models.SubmissionDTO;

public class SubmissionFileDownloadDto
{
    public required byte[] FileBytes { get; set; }

    public required string ContentType { get; set; }

    public required string DownloadString { get; set; }

}