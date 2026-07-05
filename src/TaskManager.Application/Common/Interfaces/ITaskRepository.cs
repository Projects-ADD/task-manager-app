using TaskManager.Domain.Entities;
using TaskAsync = System.Threading.Tasks.Task;
using Task = TaskManager.Domain.Entities.Task;
namespace TaskManager.Application.Common.Interfaces;

public interface ITaskRepository
{
    TaskAsync AddAsync(Task task);

    System.Threading.Tasks.Task<List<Task>> GetAllAsync();

    System.Threading.Tasks.Task<Task?> GetByIdAsync(Guid id);

    TaskAsync UpdateAsync(Task task);

    TaskAsync SaveChangesAsync();
}