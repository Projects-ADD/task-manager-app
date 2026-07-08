using TaskManager.Application.Features.Roles.DTOs;

namespace TaskManager.Application.Features.Users.DTOs;

public sealed class UserWithRolesDto
{
    public Guid Id { get; init; }

    public string Username { get; init; } = string.Empty;

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Avatar { get; init; } = string.Empty;

    public string AvatarBg { get; init; } = string.Empty;

    public DateTime LastSession { get; init; }

    public DateTime CreatedAt { get; init; }

    public bool IsActive { get; init; }

    public List<RoleDto> Roles { get; init; } = new List<RoleDto>();

    //public List<RoleWithPermissionsDto> Roles { get; init; } = new List<RoleWithPermissionsDto>();
}