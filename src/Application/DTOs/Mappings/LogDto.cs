using Application.DTOs.Enums;

namespace Application.DTOs.Mappings;

public class LogDto
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public ResourceDto ResourceDto { get; set; }
    public DateTime LoggingTime { get; set; }
    public string Message { get; set; }
    public LoggingLevel Level { get; set; }
    public string Source { get; set; }
}