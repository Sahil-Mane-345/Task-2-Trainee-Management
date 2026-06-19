namespace TraineeApi.Utility;

public static class SubmissionFilesValidator
{
    public static (bool status, string Message) ValidateFiles(IFormFileCollection formFiles)
    {
        int maxFileSize = 10 * 1024 * 1024;

        List<string> allowedExtensions = [".pdf", ".jpg", ".png"];

        foreach(var file in formFiles)
        {
            if( file == null)
            {
                return (false, "No file should be null");
            }

            if( file.Length > maxFileSize)
            {
                return (false, $"No file should exceed limit of {maxFileSize / ( 1024 * 1024)} MB");
            }

            if(!allowedExtensions.Contains(Path.GetExtension(file.FileName)))
            {
                return ( false, $"No file should be other than allowed extensions {string.Join(", ", allowedExtensions)}");
            }
        }
            return (true , "Validation passed");
    }
}