using TaskManager.Application.Features.Roles.DTOs;

namespace TaskManager.Application.Features.Permissions.DTOs;

public sealed class PermissionWithRoleDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public bool IsActive { get; init; }

    public List<RoleDto> Roles { get; init; } = new List<RoleDto>();
}