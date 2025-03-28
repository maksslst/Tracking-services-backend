using FluentValidation;

namespace Application.Requests;

public class CreateMetricValueRequest
{
    public int MetricId { get; set; }
    public double Value { get; set; }
}

public class CreateMetricValueRequestValidator : AbstractValidator<CreateMetricValueRequest>
{
    public CreateMetricValueRequestValidator()
    {
        RuleFor(x => x.MetricId).NotNull()
            .GreaterThan(0).WithMessage("MetricId must be positive")
            .LessThan(int.MaxValue).WithMessage("MetricId is too long");
        RuleFor(x => x.Value).NotNull()
            .LessThan(double.MaxValue).WithMessage("Value is too long");
    }
}