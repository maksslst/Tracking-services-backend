namespace Domain.Entities;

public class ServiceTask
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public Service Service { get; set; }
    public string Description { get; set; }
    public int? AssignedUserId { get; set; }
    public User? AssignedUser { get; set; }
    public int CreatedById { get; set; }
    public User CreatedBy { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime CompletionTime { get; set; }
    public TaskStatus Status { get; set; }
    public enum TaskStatus
    {
        Opened,
        InProgress,
        Completed
    }
}