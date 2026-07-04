using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly TaskManagerDbContext _db;

    public RoleRepository(TaskManagerDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task AddAsync(Role role)
    {
        await _db.Roles.AddAsync(role);
    }

    public async System.Threading.Tasks.Task<List<Role>> GetAllAsync()
    {
        return await _db.Roles
            .Where(r => r.DeletedAt == null)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task<Role?> GetByIdAsync(Guid id)
    {
        return await _db.Roles
            .FirstOrDefaultAsync(
                p => p.Id == id && p.DeletedAt == null);
    }

    public System.Threading.Tasks.Task UpdateAsync(Role role)
    {
        _db.Roles.Update(role);

        return System.Threading.Tasks.Task.CompletedTask;
    }

    public async System.Threading.Tasks.Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task<Role?> GetByIdWithPermissionsAsync(Guid roleId)
    {
        return await _db.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == roleId && r.DeletedAt == null);
    }

    /* public async System.Threading.Tasks.Task AddRolePermissionAsync(RolePermission rolePermission)
    {
        await _db.RolePermissions.AddAsync(rolePermission);
    } */
}