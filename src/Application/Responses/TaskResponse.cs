using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Responses;

public class TaskResponse
{
    public int ResourceId { get; set; }
    public string? Description { get; set; }
    public int? AssignedUserId { get; set; }
    public int CreatedById { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? CompletionTime { get; set; }
    public TaskStatus Status { get; set; }
}