using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Features.Permissions.DTOs;
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

    public async System.Threading.Tasks.Task AssignManyPermissionsAsync(Guid roleId, List<Guid> permissionIds)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(roleId);

        if (role is null)
        {
            throw new NotFoundException($"Role '{roleId}' was not found.", "role_not_found");
        }

        // Validate that all permissions exist before assigning any
        foreach (var permissionId in permissionIds)
        {
            var permission = await _permissionRepository.GetByIdAsync(permissionId);

            if (permission is null)
            {
                throw new NotFoundException($"Permission '{permissionId}' was not found.", "permission_not_found");
            }

            try
            {
                role.AssignPermission(permissionId);
            }
            catch (InvalidOperationException ex)
            {
                throw new ConflictException(ex.Message, "permission_already_assigned");
            }
        }

        await _roleRepository.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task AssignPermissionAsync(Guid roleId, Guid permissionId)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(roleId);

        if (role is null)
        {
            throw new NotFoundException($"Role '{roleId}' was not found.", "role_not_found");  
        }

        var permission = await _permissionRepository.GetByIdAsync(permissionId);

        if (permission is null)
        {
            throw new NotFoundException($"Permission '{permissionId}' was not found.", "permission_not_found");
        }

        try
        {
            role.AssignPermission(permissionId);
        }
        catch (InvalidOperationException ex)
        {
            throw new ConflictException( ex.Message, "permission_already_assigned");
        }


        //var rolePermission = new RolePermission( roleId, permissionId );

        //await _roleRepository.AddRolePermissionAsync(rolePermission);
        //await _roleRepository.UpdateAsync(role);
        await _roleRepository.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task RevokePermissionAsync(Guid roleId, Guid permissionId)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(roleId);

        if (role is null)
        {
            throw new NotFoundException($"Role '{roleId}' was not found.", "role_not_found");
        }

        var permission = await _permissionRepository.GetByIdAsync(permissionId);

        if (permission is null)
        {
            throw new NotFoundException($"Permission '{permissionId}' was not found.", "permission_not_found");
        }

        try
        {
            role.RevokePermission(permissionId);
        }
        catch (InvalidOperationException ex)
        {
            throw new ConflictException(ex.Message, "permission_not_assigned");
        }

        await _roleRepository.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task RevokeManyPermissionsAsync(Guid roleId, List<Guid> permissionIds)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(roleId);

        if (role is null)
        {
            throw new NotFoundException($"Role '{roleId}' was not found.", "role_not_found");
        }

        // Validate that all permissions exist before revoking any
        foreach (var permissionId in permissionIds)
        {
            var permission = await _permissionRepository.GetByIdAsync(permissionId);

            if (permission is null)
            {
                throw new NotFoundException($"Permission '{permissionId}' was not found.", "permission_not_found");
            }

            try
            {
                role.RevokePermission(permissionId);
            }
            catch (InvalidOperationException ex)
            {
                throw new ConflictException(ex.Message, "permission_not_assigned");
            }
        }

        await _roleRepository.SaveChangesAsync();
    }

    public async Task<List<PermissionDto>> GetPermissionsByRoleAsync(Guid roleId)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(roleId);

        if (role is null)
        {
            throw new NotFoundException($"Role '{roleId}' was not found.", "role_not_found");
        }

        return role.RolePermissions
            .Select(rp => new PermissionDto
            {
                Id = rp.Permission.Id,
                Name = rp.Permission.Name,
                Description = rp.Permission.Description,
                CreatedAt = rp.Permission.CreatedAt,
                IsActive = rp.Permission.IsActive
            })
            .ToList();
    }

    public async Task<List<RoleWithPermissionsDto>> GetAllWithPermissionsAsync()
    {
        var roles = await _roleRepository.GetAllWithPermissionsAsync();

        return roles.Select(r => new RoleWithPermissionsDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            CreatedAt = r.CreatedAt,
            IsActive = r.IsActive,
            Permissions = r.RolePermissions
                .Select(rp => new PermissionDto
                {
                    Id = rp.Permission.Id,
                    Name = rp.Permission.Name,
                    Description = rp.Permission.Description,
                    CreatedAt = rp.Permission.CreatedAt,
                    IsActive = rp.Permission.IsActive
                })
                .ToList()
        }).ToList();
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