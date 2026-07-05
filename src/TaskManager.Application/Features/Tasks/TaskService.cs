using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Features.Tasks.DTOs;
using TaskManager.Domain.Entities;

using TaskPriority = TaskManager.Domain.Enums.TaskPriority;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;
using Task = TaskManager.Domain.Entities.Task;

namespace TaskManager.Application.Features.Tasks;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async System.Threading.Tasks.Task<TaskDto> CreateAsync(string title, string description, TaskStatus status, TaskPriority priority, DateTime dueAt)
    {
        var task = new Task(title, description, status, priority, dueAt);

        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();

        return MapToDto(task);
    }

    public async System.Threading.Tasks.Task<List<TaskDto>> GetAllAsync()
    {
        var tasks = await _taskRepository.GetAllAsync();

        return tasks
            .Select(MapToDto)
            .ToList();
    }

    public async System.Threading.Tasks.Task<TaskDto?> GetByIdAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if ( task is null )
        {
            return null;
        }

        return MapToDto(task);
    }

    public async System.Threading.Tasks.Task<bool> UpdateAsync(Guid id, string title, string description, TaskStatus status, TaskPriority priority, DateTime dueAt)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task is null)
        {
            return false;
        }

        task.Update(title, description, status, priority, dueAt);

        await _taskRepository.SaveChangesAsync();

        return true;
    }

    public async System.Threading.Tasks.Task<bool> CompleteAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task is null)
        {
            return false;
        }

        task.Complete();

        await _taskRepository.SaveChangesAsync();

        return true;
    }

    public async System.Threading.Tasks.Task<bool> CancelAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task is null)
        {
            return false;
        }

        task.Cancel();

        await _taskRepository.SaveChangesAsync();

        return true;
    }

    public async System.Threading.Tasks.Task<bool> ReopenAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task is null)
        {
            return false;
        }

        task.Reopen();

        await _taskRepository.SaveChangesAsync();

        return true;
    }

    public async System.Threading.Tasks.Task<bool> DeleteAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task is null)
        {
            return false;
        }

        task.Delete();

        await _taskRepository.SaveChangesAsync();

        return true;
    }

    private TaskDto MapToDto(Task task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueAt = task.DueAt,
            CreatedAt = task.CreatedAt,
            IsActive = task.IsActive
        };
    }
}