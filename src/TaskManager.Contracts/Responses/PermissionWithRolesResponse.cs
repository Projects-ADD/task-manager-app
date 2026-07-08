namespace TaskManager.Contracts.Responses;

public sealed class PermissionWithRolesResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public bool IsActive { get; init; }

    public List<RoleResponse> Roles { get; init; } = new List<RoleResponse>();
}