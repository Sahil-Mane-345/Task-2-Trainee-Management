using System.Collections.ObjectModel;
using TraineeApi.MessageBroker.Entity;
using TraineeApi.Models;
using TraineeApi.Models.SubmissionDTO;

namespace TraineeApi.Services.Interfaces;

public interface IProcessingJobService
{
    Task<List<ProcessingJob>> GetAll();

    Task<string> GetStatusById(Guid Id);

    Task<ProcessingJob> RetryJob(Guid processingJobId);

    Task<ProcessingJob> CreateJob(Guid submissionFileId);
}