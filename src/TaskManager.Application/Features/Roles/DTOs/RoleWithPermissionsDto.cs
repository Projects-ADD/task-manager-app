using TaskManager.Application.Features.Permissions.DTOs;

namespace TaskManager.Application.Features.Roles.DTOs;

public sealed class RoleWithPermissionsDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public bool IsActive { get; init; }

    //public List<PermissionDto> Permissions { get; init; } = [];
    public List<PermissionDto> Permissions { get; init; } = new List<PermissionDto>();
}
