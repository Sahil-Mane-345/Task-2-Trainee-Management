namespace TraineeApi.Models.SubmissionDTO;

public class SubmissionProcessingRequestDto
{
    public Guid MessageId { get; set; } = Guid.NewGuid();

    public Guid CorrelationId { get; set; } = Guid.NewGuid();

    public required Guid SubmissionId { get; set; }

    public required Guid FileId { get; set; }

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public string ContractVersion { get; set; } = "v1";
}