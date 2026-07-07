using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public class User : AggregateRoot
{
    public string Username { get; private set; }

    public string FullName { get; private set; }

    public string Email { get; private set; }

    private string PassHash { get; set; }

    public string? Avatar { get; private set; }

    public string? AvatarBg { get; private set; }

    public DateTime LastSession { get; private set; }

    public ICollection<UserRoles> UserRoles { get; private set; } = new List<UserRoles>();

    //private readonly List<Role> _roles = [];

    //public IReadOnlyCollection<Role> Roles => _roles;

    private User()
    {
        Username = null!;
        FullName = null!;
        Email = null!;
        PassHash = null!;
    } // For EF Core

    public User(string username, string fullName, string email, string passHash)
    {
        Id = Guid.NewGuid();
        Username = username;
        FullName = fullName;
        Email = email;
        PassHash = passHash;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void Update(string username, string fullName, string email)
    {
        Username = username;
        FullName = fullName;
        Email = email;
    }

    public void UpdateAll(string username, string fullName, string email, string avatar, string avatarBg)
    {
        Username = username;
        FullName = fullName;
        Email = email;
        Avatar = avatar;
        AvatarBg = avatarBg;
    }

    public void UpdateAvatar(string avatar, string avatarBg)
    {
        Avatar = avatar;
        AvatarBg = avatarBg;
    }

    public void UpdateLastSession(DateTime lastSession)
    {
        LastSession = lastSession;
    }

    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string newPassHash)
    {
        PassHash = newPassHash;
    }

    public void AssignRole(Guid roleId)
    {
        bool alreadyAssigned = UserRoles.Any(ur => ur.RoleId == roleId);

        if (alreadyAssigned)
        {
            throw new InvalidOperationException("Role is already assigned to the user.");
        }

        UserRoles.Add(
            new UserRoles(
                Id,
                roleId
            )
        );
    }

    public void RemoveRole(Role role)
    {
        
    }

    public bool HasPermission(string permissionCode)
    {
        return false;
    }
}