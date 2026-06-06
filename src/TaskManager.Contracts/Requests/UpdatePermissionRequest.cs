namespace TaskManager.Contracts.Requests;

public sealed class UpdatePermissionRequest
{
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
}