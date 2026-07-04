namespace TaskManager.Application.Features.Users.DTOs;

public sealed class UserDto
{
    public Guid Id { get; init; }

    public string Username { get; init; } = string.Empty;

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string? Avatar { get; init; }

    public string? AvatarBg { get; init; }

    public DateTime LastSession { get; init; }

    public DateTime CreatedAt { get; init; }

    public bool IsActive { get; init; }
}