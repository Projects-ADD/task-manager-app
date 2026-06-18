namespace TaskManager.Contracts.Requests;

//¿qué quieres que la API reciba para crear el rol?
public sealed class CreateRoleRequest
{
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
}