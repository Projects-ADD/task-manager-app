namespace TaskManager.Contracts.Responses;

public sealed class UserResponse
{
    public Guid Id { get; init; }

    public string Username { get; init; } = string.Empty;

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Avatar { get; init; } = string.Empty;

    public string AvatarBg { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public bool IsActive { get; init; }
}