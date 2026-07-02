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

    public void AssignPermission(Guid permissionId)
    {
        bool alreadyAssigned = RolePermissions.Any(rp => rp.PermissionId == permissionId);

        if (alreadyAssigned)
        {
            throw new InvalidOperationException("Permission is already assigned to the role.");
        }

        RolePermissions.Add(
            new RolePermission(
                Id,
                permissionId
            )
        );
    }

    public void RevokePermission(Guid permissionId)
    {
        var rolePermission = RolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);

        if (rolePermission is null)
        {
            throw new InvalidOperationException("Permission is not assigned to the role.");
        }

        RolePermissions.Remove(rolePermission);
    }


}