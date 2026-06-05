using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

using TaskAsync = System.Threading.Tasks.Task;

namespace TaskManager.Infrastructure.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly TaskManagerDbContext _db;

    public PermissionRepository(TaskManagerDbContext db)
    {
        _db = db;
    }

    public async TaskAsync AddAsync(Permission permission)
    {
        await _db.Permissions.AddAsync(permission);
    }

    public async System.Threading.Tasks.Task<List<Permission>> GetAllAsync()
    {
        return await _db.Permissions
            .Where(p => p.DeletedAt == null)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task<Permission?> GetByIdAsync(Guid id)
    {
        return await _db.Permissions
            .FirstOrDefaultAsync(
                p => p.Id == id && p.DeletedAt == null);
    }

    public TaskAsync UpdateAsync(Permission permission)
    {
        _db.Permissions.Update(permission);

        return TaskAsync.CompletedTask;
    }

    public async TaskAsync SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}