using TaskManager.Application.Features.Permissions.DTOs;
using TaskManager.Application.Features.Roles.DTOs;

public interface IRoleService
{
    Task<RoleDto> CreateAsync(string name, string description);

    Task<List<RoleDto>> GetAllAsync();

    Task<RoleDto?> GetByIdAsync(Guid id);

    Task<bool> UpdateAsync(Guid id, string name, string description);

    Task<bool> DeleteAsync(Guid id);

    Task AssignPermissionAsync(Guid roleId, Guid permissionId);

    Task AssignManyPermissionsAsync(Guid roleId, List<Guid> permissionIds);

    Task RevokePermissionAsync(Guid roleId, Guid permissionId);

    Task<List<PermissionDto>> GetPermissionsByRoleAsync(Guid roleId);
}