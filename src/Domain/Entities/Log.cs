using Domain.Enums; 

namespace Domain.Entities;

public class Log
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public Resource Resource { get; set; }
    public DateTime LoggingTime { get; set; }
    public string Message { get; set; }
    public LoggingLevel Level { get; set; }
    public string Source { get; set; }
}