using Domain.Enums;

namespace Domain.Entities;

public class Log
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public Resource? Resource { get; set; }
    public DateTime LoggingTime { get; set; }
    public required string Message { get; set; }
    public LoggingLevel Level { get; set; }
    public required string Source { get; set; }
}