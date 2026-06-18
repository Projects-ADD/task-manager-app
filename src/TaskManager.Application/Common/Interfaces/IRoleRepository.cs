using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Interfaces;

public interface IRoleRepository
{
    System.Threading.Tasks.Task AddAsync(Role role);

    System.Threading.Tasks.Task<List<Role>> GetAllAsync();

    System.Threading.Tasks.Task<Role?> GetByIdAsync(Guid id);

    System.Threading.Tasks.Task UpdateAsync(Role role);

    System.Threading.Tasks.Task SaveChangesAsync();
}