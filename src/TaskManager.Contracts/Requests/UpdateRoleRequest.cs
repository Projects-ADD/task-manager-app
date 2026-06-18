namespace TaskManager.Contracts.Requests;

//¿qué quieres que la API reciba para actualizar el rol?
public sealed class UpdateRoleRequest
{
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
}