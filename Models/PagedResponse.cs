namespace TraineeApi.Models;

public class PagedResponse<T>
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public int TotalRecords { get; set; } = 0;

    public T? Data { get; set; }
} 