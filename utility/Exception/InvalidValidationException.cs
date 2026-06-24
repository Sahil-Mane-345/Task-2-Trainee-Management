namespace TraineeApi.Utility.Exception;
using System;
public class InvalidValidationException: Exception
{
    public InvalidValidationException(string message) : base(message)
    {
        
    }
}