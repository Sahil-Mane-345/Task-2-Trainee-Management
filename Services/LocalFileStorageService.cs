using Microsoft.EntityFrameworkCore;
using TraineeApi.Context;
using TraineeApi.Models;
using TraineeApi.Models.Entity;
using TraineeApi.Models.SubmissionDTO;
using TraineeApi.Services.Interfaces;
using TraineeApi.Utility;
using TraineeApi.Utility.Exception;

namespace TraineeApi.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly AppDbContext _context;

    private readonly IConfiguration _configuration;

    private readonly IProcessingJobService _processingJobService;

    public LocalFileStorageService( IConfiguration configuration, AppDbContext context , IProcessingJobService processingJobService)
    {
        _configuration = configuration;
        _context = context;
        _processingJobService = processingJobService;
    }

    public async Task<ApiResponse<object>> SaveAsync(Guid submissionId, Guid UserId,IFormFileCollection FormFiles)
    {
        bool submissionExists = await _context.Submissions.AnyAsync( s => s.Id == submissionId);

        if( !submissionExists )
        {
            throw new NotFoundException("No such submission Id exists ");
        }

        ApiResponse<object> res = new();

        SubmissionFilesValidator.ValidateFiles(FormFiles);

        List<SubmissionFile> submissionFiles = [];

        foreach(var file in FormFiles)
        {
            Guid fileId = Guid.NewGuid();
            string fileName = $"{fileId.ToString()}{Path.GetExtension(file.FileName)}";
            string FilePath = Path.Combine(_configuration["FilePaths:SubmissionFilePath"]!, fileName);

            string checksum = "";

            using( var stream = File.Create(FilePath))
            {
                await file.CopyToAsync(stream);
            }


            SubmissionFile submissionFile = new()
            {
                Id = fileId,
                GeneratedFileName = fileName,
                OriginalFileName = file.FileName,
                SubmissionId = submissionId,
                ContentType = file.ContentType,
                Size = file.Length,
                CheckSum = checksum,
                UserId = UserId,
            };

            await _context.AddAsync(submissionFile);
            await _context.SaveChangesAsync();
            await _processingJobService.CreateJob(fileId);
            submissionFiles.Add(submissionFile);
        }
        
        res.Success = true;
        res.Message = "All Files uploaded successfully.";
        res.Data = submissionFiles;

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
            throw new NotFoundException("Metadata not found for this Id");
        }

        string FilePath = Path.Combine(_configuration["FilePaths:SubmissionFilePath"]!, submissionFile.GeneratedFileName);
        if( !File.Exists(FilePath) )
        {
            throw new NotFoundException("File not found in storage");
        }

        byte[] filesBytes = await File.ReadAllBytesAsync(FilePath);

        res.Success = true;
        res.Message = "File found";
        res.Data = new SubmissionFileDownloadDto{
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
            throw new NotFoundException("Metadata not found for this Id");
        }
        
        string FilePath = Path.Combine(_configuration["FilePaths:SubmissionFilePath"]!, submissionFile.GeneratedFileName);
        File.Delete(FilePath);

        _context.Remove(submissionFile);
        await _context.SaveChangesAsync();
        return true;
    }
}