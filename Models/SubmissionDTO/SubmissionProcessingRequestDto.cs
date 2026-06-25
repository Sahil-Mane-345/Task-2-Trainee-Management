namespace TraineeApi.Models.SubmissionDTO;

public class SubmissionProcessingRequestDto
{
    public required Guid SubmissionFileId { get; set; }

    public string ContractVersion { get; set; } = "v1";
}