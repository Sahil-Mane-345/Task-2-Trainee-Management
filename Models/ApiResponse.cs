namespace TraineeApi.Models;

public class ApiResponse<T>{
    public bool Success {get; set;}

    public string? message {get; set;} = string.Empty;

    public T? Data {get; set;}

}