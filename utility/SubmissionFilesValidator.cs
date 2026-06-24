using TraineeApi.Utility.Exception;

namespace TraineeApi.Utility;

public static class SubmissionFilesValidator
{
    public static void ValidateFiles(IFormFileCollection formFiles)
    {
        int maxFileSize = 10 * 1024 * 1024;

        List<string> allowedExtensions = [".pdf", ".jpg", ".png"];

        foreach(var file in formFiles)
        {
            if( file == null)
            {
                throw new InvalidFileValidationException("File should not be null");
            }

            if( file.Length > maxFileSize)
            {
                throw new InvalidFileValidationException($"No file should exceed limit of {maxFileSize / ( 1024 * 1024)} MB");
            }

            if(!allowedExtensions.Contains(Path.GetExtension(file.FileName)))
            {
                throw new InvalidFileValidationException( $"No file should be other than allowed extensions {string.Join(", ", allowedExtensions)}");
            }
        }
            
    }
}