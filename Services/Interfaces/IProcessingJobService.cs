
using TraineeApi.MessageBroker.Entity;


namespace TraineeApi.Services.Interfaces;

public interface IProcessingJobService
{
    Task<List<ProcessingJob>> GetAll();

    Task<string> GetStatusById(Guid Id);

    Task<ProcessingJob> RetryJob(Guid processingJobId);

    Task<ProcessingJob> CreateJob(Guid submissionFileId);
}