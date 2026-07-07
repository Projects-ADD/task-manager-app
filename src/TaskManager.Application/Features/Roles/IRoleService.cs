using TaskManager.Application.Features.Permissions.DTOs;
using TaskManager.Application.Features.Roles.DTOs;

namespace TaskManager.Application.Features.Roles;
public interface IRoleService
{
    Task<RoleDto> CreateAsync(string name, string description);

    Task<List<RoleDto>> GetAllAsync();

    Task<RoleDto?> GetByIdAsync(Guid id);

    Task<bool> UpdateAsync(Guid id, string name, string description);

    Task<bool> DeleteAsync(Guid id);

    Task AssignPermissionAsync(Guid roleId, Guid permissionId);

    Task AssignManyPermissionsAsync(Guid roleId, List<Guid> permissionIds);

    Task AssignManyUsersAsync(Guid roleId, List<Guid> userIds);

    Task RevokePermissionAsync(Guid roleId, Guid permissionId);

    Task RevokeManyPermissionsAsync(Guid roleId, List<Guid> permissionIds);

    Task<List<PermissionDto>> GetPermissionsByRoleAsync(Guid roleId);

    Task<List<RoleWithPermissionsDto>> GetAllWithPermissionsAsync();

    Task<RoleWithPermissionsDto?> GetByIdWithPermissionsAsync(Guid id);
}