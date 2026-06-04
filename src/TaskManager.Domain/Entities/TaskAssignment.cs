public class TaskAssignment : BaseEntity
{
    public Guid TaskId { get; private set; }

    public Guid UserId { get; private set; }

    public AssignmentRole AssignmentRole { get; private set; }
}