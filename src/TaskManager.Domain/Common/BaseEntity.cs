namespace TaskManager.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }

    public DateTime CreatedAt { get; protected set; }

    public DateTime DeletedAt { get; protected set; }

    public Guid DeletedBy { get; protected set; }

    public bool IsActive { get; protected set; }
}