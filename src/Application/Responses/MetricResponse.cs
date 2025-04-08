namespace Application.Responses;

public class MetricResponse
{
    public string Name { get; set; }
    public int ResourceId { get; set; }
    public DateTime Created { get; set; }
    public string Unit { get; set; }
}