using FileSignatures;
using TraineeApi.Utility.Exception;

namespace TraineeApi.Utility;

public static class SubmissionFilesValidator
{
    public static void ValidateFiles(IFormFileCollection formFiles)
    {
        int maxFileSize = 10 * 1024 * 1024;

        HashSet<string> allowedContentTypes = ["application/pdf","image/jpeg","image/png"];

        HashSet<string> allowedExtensions = [".pdf", ".jpg", ".png"];

        foreach(var file in formFiles)
        {
            if( file == null || file.Length == 0)
            {
                throw new InvalidFileValidationException("File should not be null");
            }

            if( file.Length > maxFileSize)
            {
                throw new InvalidFileValidationException($"No file should exceed limit of {maxFileSize / ( 1024 * 1024)} MB");
            }

            if(!allowedExtensions.Contains(Path.GetExtension(file.FileName)))
            {
                throw new InvalidFileValidationException( $"File extension {Path.GetExtension(file.FileName)} not allowed");
            }

            if (!allowedContentTypes.Contains(file.ContentType))
            {
                throw new InvalidFileValidationException($"File Content-type {file.ContentType} not allowed");
            }

            using var stream = file.OpenReadStream();
            var inspector = new FileFormatInspector();

            var format = inspector.DetermineFileFormat(stream) ?? throw new InvalidFileValidationException("Unable to determine type");
            
            string detectedExtension = "." + format.Extension.ToLowerInvariant();

            if(!string.Equals(detectedExtension, Path.GetExtension(file.FileName), StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidFileValidationException("File signature does not match the file extension");
            }
        }
            
    }
}