using TaskStatus = Application.DTOs.Enums.TaskStatus;

namespace Application.DTOs.Mappings;

public class ServiceTaskDto
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public ResourceDto Resource { get; set; }
    public string Description { get; set; }
    public int? AssignedUserId { get; set; }
    public UserDto? AssignedUser { get; set; }
    public int CreatedById { get; set; }
    public UserDto CreatedBy { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? CompletionTime { get; set; }
    public TaskStatus Status { get; set; }
}