namespace TraineeApi.Utility.Exception;
using System;
public class InvalidIdentifierException: Exception
{
    public InvalidIdentifierException(string message) : base(message)
    {
        
    }
}