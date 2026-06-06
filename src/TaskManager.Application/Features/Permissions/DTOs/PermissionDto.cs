namespace TaskManager.Application.Features.Permissions.DTOs;

//Solo los datos que la capa de aplicación necesita exponer hacia afuera del caso de uso
public sealed class PermissionDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public bool IsActive { get; init; }
}