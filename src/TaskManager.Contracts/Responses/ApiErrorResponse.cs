namespace TaskManager.Application.Common.Exceptions;

public sealed class ApiErrorResponse
{
    public string Action { get; set; } = string.Empty;
    public int HttpStatusCode { get; set; }
    public string ErrorCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;
    public object? Data { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}