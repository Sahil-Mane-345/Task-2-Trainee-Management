using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TraineeApi.Context;
using TraineeApi.MessageBroker;
using TraineeApi.MessageBroker.Services;
using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.SubmissionDTO;
using TraineeApi.Services.Interfaces;
using TraineeApi.Utility;

namespace TraineeApi.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly AppDbContext _context;

    private readonly IConfiguration _configuration;

    private readonly HttpContext _httpContext;
    private readonly IRabbitMQPublisher _rabbitMQPublisher;

    public LocalFileStorageService( IConfiguration configuration, AppDbContext context , IHttpContextAccessor httpContextAccessor, IRabbitMQPublisher rabbitMQPublisher)
    {
        _configuration = configuration;
        _context = context;
        _httpContext = httpContextAccessor.HttpContext!;
        _rabbitMQPublisher = rabbitMQPublisher;
    }

    public async Task<ApiResponse<object>> SaveAsync(Guid submissionId, IFormFileCollection FormFiles)
    {
        bool submissionExists = await _context.Submissions.AnyAsync( s => s.Id == submissionId);

        if( !submissionExists )
        {
            throw new ArgumentException("No such submission Id exists ");
        }

        ApiResponse<object> res = new();

        var result = SubmissionFilesValidator.ValidateFiles(FormFiles);
        if (!result.status)
        {
            res.success = false;
            res.message = result.Message;
            return res;
        }


        List<SubmissionFile> submissionFiles = new List<SubmissionFile>();

        foreach(var file in FormFiles)
        {
            Guid fileId = Guid.NewGuid();
            string fileName = $"{fileId.ToString()}{Path.GetExtension(file.FileName)}";
            string FilePath = Path.Combine(_configuration["FilePaths:SubmissionFilePath"]!, fileName);

            string checksum = "";

            using( var sha = SHA256.Create())
            using( var stream = File.Create(FilePath))
            {
                await file.CopyToAsync(stream);
                var hashBytes = await sha.ComputeHashAsync(file.OpenReadStream());
                checksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                Console.WriteLine($"Checksum : {checksum}");
            }

            var userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            Guid guid = new(userId!);
            SubmissionFile submissionFile = new()
            {
                Id = fileId,
                GeneratedFileName = fileName,
                OriginalFileName = file.FileName,
                SubmissionId = submissionId,
                ContentType = file.ContentType,
                Size = file.Length,
                CheckSum = checksum,
                UserId = new Guid(userId!),
            };

            await _context.AddAsync(submissionFile);
            await _context.SaveChangesAsync();
            submissionFiles.Add(submissionFile);

            SubmissionProcessingRequestDto submissionProcessingRequestDto = new()
            {
                SubmissionId = submissionId,
                FileId = submissionFile.Id,
            };

            await _rabbitMQPublisher.PublishMessageAsync(submissionProcessingRequestDto, RabbitMQQueues.SubmissionProcessing);

        }
        
        res.success = true;
        res.message = "All Files uploaded successfully.";
        res.data = submissionFiles;

        return res;
    }

    public async Task<bool> ExistsAsync(Guid submissionFileId)
    {
        SubmissionFile? submissionFile = await _context.SubmissionFiles.FindAsync(submissionFileId);
        if( submissionFile == null)
        {
            return false;
        } 

        string  FilePath = Path.Combine(_configuration["FilePaths:SubmissionFilePath"]!, submissionFile.GeneratedFileName);
        if(File.Exists(FilePath))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public async Task<ApiResponse<SubmissionFileDownloadDto>> OpenReadAsync(Guid submissionFileId)
    {
        ApiResponse<SubmissionFileDownloadDto> res = new();
        SubmissionFile? submissionFile = await _context.SubmissionFiles.FindAsync(submissionFileId);

        if( submissionFile == null)
        {
            res.success = false;
            res.message = "Metadata not found for this Id";
            
            return res;
        }

        string FilePath = Path.Combine(_configuration["FilePaths:SubmissionFilePath"]!, submissionFile.GeneratedFileName);
        if( !File.Exists(FilePath) )
        {
            res.success = false;
            res.message = "File not found in that path";
            
            return res;
        }

        byte[] filesBytes = await File.ReadAllBytesAsync(FilePath);

        res.success = true;
        res.message = "File found";
        res.data = new SubmissionFileDownloadDto{
            FileBytes = filesBytes,
            ContentType = submissionFile.ContentType,
            DownloadString = submissionFile.GeneratedFileName
        };
        
        return res;
    }

    public async Task<bool> DeleteAsync(Guid submissionFileId)
    {
        SubmissionFile? submissionFile = await _context.SubmissionFiles.FindAsync(submissionFileId);

        if( submissionFile == null)
        {
            return false;
        }

        string FilePath = Path.Combine(_configuration["FilePaths:SubmissionFilePath"]!, submissionFile.GeneratedFileName);
        File.Delete(FilePath);

        _context.Remove(submissionFile);
        await _context.SaveChangesAsync();
        return true;
    }
}