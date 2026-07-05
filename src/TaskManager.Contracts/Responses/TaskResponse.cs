using TaskManager.Contracts.Enums;

namespace TaskManager.Contracts.Responses;

public sealed class TaskResponse
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public TaskStatusContract Status { get; init; }

    public TaskPriorityContract Priority { get; init; }

    public DateTime DueAt { get; init; }

    public DateTime CreatedAt { get; init; }
}