public class Task : AggregateRoot
{
    public string Title { get; private set; }

    public string Description { get; private set; }

    public TaskStatus Status { get; private set; }

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