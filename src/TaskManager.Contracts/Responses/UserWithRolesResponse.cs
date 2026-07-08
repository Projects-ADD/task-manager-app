namespace TaskManager.Contracts.Responses;

public sealed class UserWithRolesResponse
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

    public List<RoleResponse> Roles { get; init; } = new List<RoleResponse>();
}