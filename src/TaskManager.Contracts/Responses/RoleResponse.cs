namespace TaskManager.Contracts.Responses;

//¿con qué datos deseas que la API responda?
public sealed class RoleResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public bool IsActive { get; init; }

}