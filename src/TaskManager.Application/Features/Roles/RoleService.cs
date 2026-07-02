using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Features.Roles.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Roles;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public RoleService(IRoleRepository roleRepository, IPermissionRepository permissionRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<RoleDto> CreateAsync(string name, string description)
    {
        var role = new Role(name, description);

        await _roleRepository.AddAsync(role);
        await _roleRepository.SaveChangesAsync();

        return MapToDto(role);
    }

    public async Task<List<RoleDto>> GetAllAsync()
    {
        var roles = await _roleRepository.GetAllAsync();

        return roles
            .Select(MapToDto)
            .ToList();
    }

    public async Task<RoleDto?> GetByIdAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);

        if ( role is null )
        {
            return null;
        }

        return MapToDto(role);
    }

    public async Task<bool> UpdateAsync(Guid id, string name, string description)
    {
        var role = await _roleRepository.GetByIdAsync(id);

        if (role is null)
        {
            return false;
        }

        role.Update(name, description);

        await _roleRepository.UpdateAsync(role);
        await _roleRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);

        if (role is null)
        {
            return false;
        }

        role.Delete();

        await _roleRepository.UpdateAsync(role);
        await _roleRepository.SaveChangesAsync();

        return true;
    }

    public async System.Threading.Tasks.Task AssignPermissionAsync(Guid roleId, Guid permissionId)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(roleId);

        if (role is null)
        {
            throw new Exception("Role not found");
        }

        var permission = await _permissionRepository.GetByIdAsync(permissionId);

        if (permission is null)
        {
            throw new Exception("Permission not found");
        }

        role.AssignPermission(permissionId);

        //var rolePermission = new RolePermission( roleId, permissionId );

        //await _roleRepository.AddRolePermissionAsync(rolePermission);
        //await _roleRepository.UpdateAsync(role);
        await _roleRepository.SaveChangesAsync();
    }

    private static RoleDto MapToDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            CreatedAt = role.CreatedAt,
            IsActive = role.IsActive
        };
    }

}