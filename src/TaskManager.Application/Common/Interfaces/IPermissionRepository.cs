using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Interfaces;

public interface IPermissionRepository
{
    Task AddAsync(Permission permission);

    Task<List<Permission>> GetAllAsync();

    Task<Permission?> GetByIdAsync(Guid id);

    Task UpdateAsync(Permission permission);

    Task SaveChangesAsync();
}

