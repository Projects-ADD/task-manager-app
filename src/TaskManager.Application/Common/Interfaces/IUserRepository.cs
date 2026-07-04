using TaskManager.Domain.Entities;
using TaskAsync = System.Threading.Tasks.Task;
namespace TaskManager.Application.Common.Interfaces;

public interface IUserRepository
{
    TaskAsync AddAsync(User user);

    System.Threading.Tasks.Task<List<User>> GetAllAsync();

    System.Threading.Tasks.Task<User?> GetByIdAsync(Guid id);

    TaskAsync UpdateAsync(User user);

    TaskAsync SaveChangesAsync();
}