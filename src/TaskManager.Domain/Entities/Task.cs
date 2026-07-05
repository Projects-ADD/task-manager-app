using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;

using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.Domain.Entities;

public class Task : AggregateRoot
{
    public string Title { get; private set; }

    public string Description { get; private set; }

    public TaskStatus Status { get; private set; }

    public TaskPriority Priority { get; private set; }

    public DateTime DueAt { get; private set; }

    private Task()
    {
        Title = null!;
        Description = null!;
    } // For EF Core

    public Task(string title, string description, TaskStatus status, TaskPriority priority, DateTime dueAt)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
        DueAt = dueAt;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void Update(string title, string description, TaskStatus status, TaskPriority priority, DateTime dueAt)
    {
        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
        DueAt = dueAt;
    }

    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;
        IsActive = false;
    }

    public void UpdateStatus(TaskStatus status)
    {
        Status = status;
    } 

    public void Complete()
    {
        Status = TaskStatus.Completed;
    }

    public void Cancel()
    {
        Status = TaskStatus.Cancelled;
    }

    public void Reopen()
    {
        Status = TaskStatus.InProgress;
    }
}