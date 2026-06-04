using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public class Permission : AggregateRoot
{
    public string Name { get; private set; }

    public string Description { get; private set; }

    private Permission(){}

    public Permission(string name, string description)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;
    }
}