using TaskManager.Application.Features.Permissions.DTOs;

namespace TaskManager.Application.Features.Permissions;

public interface IPermissionService
{
    Task<PermissionDto> CreateAsync(string name, string description);
    
    Task<List<PermissionDto>> GetAllAsync();

    Task<PermissionDto?> GetByIdAsync(Guid id);

    Task<PermissionWithRoleDto?> GetOneWithRolesAsync(Guid permissionId);

    Task<bool> UpdateAsync(Guid id, string name, string description);

    Task<bool> DeleteAsync(Guid id);

}