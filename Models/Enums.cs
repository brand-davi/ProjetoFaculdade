namespace ProjectManagerMvc.Models
{
    public enum ProjectStatus
    {
        Planned = 0,
        InProgress = 1,
        Done = 2,
        Cancelled = 3
    }

    public enum TaskStatus
    {
        Pending = 0,
        InProgress = 1,
        Done = 2,
        Blocked = 3,
        Cancelled = 4
    }
}
