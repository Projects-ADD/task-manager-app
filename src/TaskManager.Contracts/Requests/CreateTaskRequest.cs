using TaskManager.Contracts.Enums;

namespace TaskManager.Contracts.Requests;

public sealed class CreateTaskRequest
{
    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public TaskStatusContract Status { get; init; }

    public TaskPriorityContract Priority { get; init; }

    public DateTime DueAt { get; init; }
}