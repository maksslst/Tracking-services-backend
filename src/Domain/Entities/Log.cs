namespace Domain.Entities;

public class Log
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public Service Service { get; set; }
    public DateTime LoggingTime { get; set; }
    public string Message { get; set; }
    public LoggingLevel Level { get; set; }
    public enum LoggingLevel
    {
        Error,
        Warning,
        Info,
        Critical
    }
    public string Source { get; set; }
}