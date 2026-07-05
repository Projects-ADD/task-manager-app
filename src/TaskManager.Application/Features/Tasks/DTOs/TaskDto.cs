namespace TaskManager.Application.Features.Tasks.DTOs;

public sealed class TaskDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public TaskManager.Domain.Enums.TaskStatus Status { get; init; }

    public TaskManager.Domain.Enums.TaskPriority Priority { get; init; }

    public DateTime DueAt { get; init; }

    public DateTime CreatedAt { get; init; }

    public bool IsActive { get; init; }
}