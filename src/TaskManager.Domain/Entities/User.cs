public class User : AggregateRoot
{
    public string Email { get; private set; }

    private string PasswordHash { get; set; }

    private readonly List<Role> _roles = [];

    public IReadOnlyCollection<Role> Roles => _roles;

    public void AssignRole(Role role)
    {
        
    }

    public void RemoveRole(Role role)
    {
        
    }

    public bool HasPermission(string permissionCode)
    {
        return false;
    }
}