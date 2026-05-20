namespace UserApi.Shared.Contracts;

public class ApiResponse<T>
{
    public int Status { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }

    public ApiResponse(int status, string message, T? data)
    {
        Status = status;
        Message = message;
        Data = data;
    }
}

public class ErrorResponse
{
    public int Status { get; set; }
    public string Error { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;

    public ErrorResponse(int status, string error, string message)
    {
        Status = status;
        Error = error;
        Message = message;
    }
}
