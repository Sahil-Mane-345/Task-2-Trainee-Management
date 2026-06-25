using Microsoft.EntityFrameworkCore;
using TraineeApi.Context;
using TraineeApi.MessageBroker.Constants;
using TraineeApi.MessageBroker.Entity;
using TraineeApi.MessageBroker.Services;
using TraineeApi.Models.Entity;
using TraineeApi.Models.SubmissionDTO;
using TraineeApi.Services.Interfaces;
using TraineeApi.Utility.Exception;

namespace TraineeApi.Services;

public class ProcessingJobService : IProcessingJobService
{
    private readonly AppDbContext _context;

    private readonly IRabbitMQPublisher _rabbitMQPublisher;

    public ProcessingJobService(AppDbContext context, IRabbitMQPublisher rabbitMQPublisher)
    {
        _context = context;
        _rabbitMQPublisher = rabbitMQPublisher;
    }

    public async Task<ProcessingJob> CreateJob(Guid submissionFileId)
    {
        SubmissionFile submissionFile = await _context.SubmissionFiles.FindAsync(submissionFileId) ?? throw new NotFoundException("Submission File Not found for this Id");

        ProcessingJob processingJob = new()
        {
            CorrelationId = Guid.NewGuid().ToString(),
            MessageId = Guid.NewGuid().ToString(),
            SubmissionFileId = submissionFileId,
            Status = "Queued"
        };

        SubmissionProcessingRequestDto submissionProcessingRequestDto = new()
        {
            SubmissionFileId = submissionFile.Id,
        };
        
        await _rabbitMQPublisher.PublishFileMessageAsync(submissionProcessingRequestDto, RabbitMQConstants.SubmissionProcessingQueue, processingJob.CorrelationId, processingJob.MessageId);

        await _context.ProcessingJobs.AddAsync(processingJob);
        await _context.SaveChangesAsync();

        return processingJob;
    }

    public async Task<List<ProcessingJob>> GetAll()
    {
        return await _context.ProcessingJobs.ToListAsync();
    }

    public async Task<string> GetStatusById(Guid Id)
    {
        ProcessingJob processingJob = await _context.ProcessingJobs.FindAsync(Id) ?? throw new NotFoundException("Process Job Not found with such Id");
        return processingJob.Status;
    }

    public async Task<ProcessingJob> RetryJob(Guid processingJobId)
    {
        ProcessingJob processingJob = await _context.ProcessingJobs.FindAsync(processingJobId) ?? throw new NotFoundException("Process Job Not found with such Id");
        if( ! processingJob.Status.Equals("Failed"))
        {
            throw new Exception("Process Job is not Failed you cant retry");
        }
        processingJob.Attempts = 0;
        processingJob.ErrorSummary = "";
        processingJob.Status = "Queued";
        processingJob.MessageId = Guid.NewGuid().ToString();

        SubmissionProcessingRequestDto submissionProcessingRequestDto = new()
        {
            SubmissionFileId = processingJob.SubmissionFileId
        };

        await _rabbitMQPublisher.PublishFileMessageAsync(submissionProcessingRequestDto, RabbitMQConstants.SubmissionProcessingQueue, processingJob.CorrelationId, processingJob.MessageId);

        await _context.SaveChangesAsync();

        return processingJob; 
    }
}