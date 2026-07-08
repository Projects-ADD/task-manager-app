using TaskManager.Domain.Entities;
using TaskAsync = System.Threading.Tasks.Task;
namespace TaskManager.Application.Common.Interfaces;

public interface IPermissionRepository
{
    TaskAsync AddAsync(Permission permission);

    System.Threading.Tasks.Task<List<Permission>> GetAllAsync();

    System.Threading.Tasks.Task<Permission?> GetByIdAsync(Guid id);

    System.Threading.Tasks.Task<Permission?> GetOneWithRolesAsync(Guid permissionId);

    TaskAsync UpdateAsync(Permission permission);

    TaskAsync SaveChangesAsync();
}

