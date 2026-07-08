using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Features.Permissions.DTOs;
using TaskManager.Application.Features.Roles.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Permissions;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleRepository _roleRepository;

    public PermissionService(IPermissionRepository permissionRepository, IRoleRepository roleRepository)
    {
        _permissionRepository = permissionRepository;
        _roleRepository = roleRepository;
    }

    public async Task<PermissionDto> CreateAsync(string name, string description)
    {
        var permission = new Permission(name, description);

        await _permissionRepository.AddAsync(permission);
        await _permissionRepository.SaveChangesAsync();

        return MapToDto(permission);
    }

    public async Task<List<PermissionDto>> GetAllAsync()
    {
        var permissions = await _permissionRepository.GetAllAsync();

        return permissions
            .Select(MapToDto)
            .ToList();
    }

    public async Task<PermissionDto?> GetByIdAsync(Guid id)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);

        if (permission is null)
        {
            return null;
        }

        return MapToDto(permission);
    }

    public async Task<PermissionWithRoleDto?> GetOneWithRolesAsync(Guid permissionId)
    {
        var permission = await _permissionRepository.GetOneWithRolesAsync(permissionId);

        if (permission is null)
        {
            return null;
        }

        return new PermissionWithRoleDto
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description,
            CreatedAt = permission.CreatedAt,
            IsActive = permission.IsActive,
            Roles = permission.RolePermissions.Select(rp => new RoleDto
            {
                Id = rp.Role.Id,
                Name = rp.Role.Name,
                Description = rp.Role.Description,
                CreatedAt = rp.Role.CreatedAt,
                IsActive = rp.Role.IsActive
            }).ToList()
        };
    }

    public async Task<bool> UpdateAsync(Guid id, string name, string description)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);

        if (permission is null)
        {
            return false;
        }

        permission.Update(name, description);

        await _permissionRepository.UpdateAsync(permission);
        await _permissionRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);

        if (permission is null)
        {
            return false;
        }

        permission.Delete();

        await _permissionRepository.UpdateAsync(permission);
        await _permissionRepository.SaveChangesAsync();

        return true;
    }

    private static PermissionDto MapToDto(Permission permission)
    {
        return new PermissionDto
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description,
            CreatedAt = permission.CreatedAt,
            IsActive = permission.IsActive
        };
    }
}