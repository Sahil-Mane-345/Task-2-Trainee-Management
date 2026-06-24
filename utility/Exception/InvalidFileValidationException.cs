namespace TraineeApi.Utility.Exception;
using System;
public class InvalidFileValidationException: Exception
{
    public InvalidFileValidationException(string message) : base(message)
    {
        
    }
}