using System.Collections.ObjectModel;
using TraineeApi.Models;
using TraineeApi.Models.SubmissionDTO;

namespace TraineeApi.Services.Interfaces;

public interface IFileStorageService
{
    Task<ApiResponse<object>> SaveAsync(Guid SUbmissionId,Guid UserId, IFormFileCollection FormFiles);

    Task<ApiResponse<SubmissionFileDownloadDto>> OpenReadAsync(Guid submissionFileId);

    Task<bool> ExistsAsync(Guid SubmissionFileId);

    Task<bool> DeleteAsync(Guid submissionFileId);
}