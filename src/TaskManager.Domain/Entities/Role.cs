using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public class Role : AggregateRoot
{
    public string Name { get; private set; }

    public string Description { get; private set; }

    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
    //private readonly List<Permission> _permissions = [];

    public Role(string name, string description)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;
    }

    //public IReadOnlyCollection<Permission> Permissions => _permissions;

    public void AddPermission(Permission permission)
    {
    
    }

    public void RemovePermission(Permission permission)
    {

    }


}