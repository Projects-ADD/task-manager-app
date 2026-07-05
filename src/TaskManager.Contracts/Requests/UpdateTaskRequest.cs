using TaskManager.Contracts.Enums;

namespace TaskManager.Contracts.Requests;

public class UpdateTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriorityContract Priority { get; set; } = TaskPriorityContract.Medium;
    public TaskStatusContract Status { get; set; } = TaskStatusContract.Pending;
    public DateTime DueAt { get; set; } = DateTime.UtcNow.AddDays(7);
}