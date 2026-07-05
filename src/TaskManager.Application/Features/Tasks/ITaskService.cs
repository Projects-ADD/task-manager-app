using TaskManager.Application.Features.Tasks.DTOs;
using TaskPriority = TaskManager.Domain.Enums.TaskPriority;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;
namespace TaskManager.Application.Features.Tasks;

// in this case, the class Task is referencig to class System.Threading.Tasks.Task
public interface ITaskService
{
    System.Threading.Tasks.Task<TaskDto> CreateAsync(string title, string description, TaskStatus status, TaskPriority priority, DateTime dueAt);

    System.Threading.Tasks.Task<List<TaskDto>> GetAllAsync();

    System.Threading.Tasks.Task<TaskDto?> GetByIdAsync(Guid id);

    System.Threading.Tasks.Task<bool> UpdateAsync(Guid id, string title, string description, TaskStatus status, TaskPriority priority, DateTime dueAt);

    System.Threading.Tasks.Task<bool> CompleteAsync(Guid id);

    System.Threading.Tasks.Task<bool> CancelAsync(Guid id);

    System.Threading.Tasks.Task<bool> ReopenAsync(Guid id);

    System.Threading.Tasks.Task<bool> DeleteAsync(Guid id);
}