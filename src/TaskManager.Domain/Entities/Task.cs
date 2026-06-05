using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public class Task : AggregateRoot
{
    public string Title { get; private set; }

    public string Description { get; private set; }

    public TaskManager.Domain.Enums.TaskStatus Status { get; private set; }

    public DateTime DueAt { get; private set; }

    public void Complete()
    {
        
    }

    public void Cancel()
    {
        
    }

    public void Reopen()
    {
        
    }
}