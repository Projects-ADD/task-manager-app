public class Role : AggregateRoot
{
    public string Name { get; private set; }

    public string Description { get; private set; }

    private readonly List<Permission> _permissions = [];

    public IReadOnlyCollection<Permission> Permissions => _permissions;

    public void AddPermission(Permission permission)
    {
    
    }

    public void RemovePermission(Permission permission)
    {

    }


}