using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public class Role : AggregateRoot
{
    public string Name { get; private set; }

    public string Description { get; private set; }

    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
    
    public ICollection<UserRoles> UserRoles { get; private set; } = new List<UserRoles>();
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

    public void AssignUser(Guid userId)
    {
        bool alreadyAssigned = UserRoles.Any(ur => ur.UserId == userId);

        if (alreadyAssigned)
        {
            throw new InvalidOperationException("User is already assigned to the role.");
        }

        UserRoles.Add(
            new UserRoles(
                userId,
                Id
            )
        );
    }

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