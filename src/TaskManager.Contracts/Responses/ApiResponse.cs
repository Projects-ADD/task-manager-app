namespace TaskManager.Contracts.Responses;

public sealed class ApiResponse<T>
{
    public string Action { get; set; } = string.Empty;

    public int HttpStatusCode { get; set; }

    public string Message { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public T? Data { get; set; } = default!;
}