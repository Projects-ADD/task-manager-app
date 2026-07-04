namespace TaskManager.Contracts.Responses;

public sealed class RoleWithPermissionsResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public bool IsActive { get; init; }

    //public List<PermissionResponse> Permissions { get; init; } = [];
    public List<PermissionResponse> Permissions { get; init; } = new List<PermissionResponse>();
}
