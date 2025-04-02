using FluentValidation;

namespace Application.Requests;

public class UpdateMetricRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int ResourceId { get; set; }
    public string Unit { get; set; } = null!;
}

public class UpdateMetricRequestValidator : AbstractValidator<UpdateMetricRequest>
{
    public UpdateMetricRequestValidator()
    {
        RuleFor(x => x.Id).NotNull()
            .GreaterThan(0).WithMessage("RequestId must be positive");
        RuleFor(x => x.Name).NotEmpty()
            .MaximumLength(100).WithMessage("{PropertyName} maximum length is 100 characters");
        RuleFor(x => x.ResourceId).NotEmpty()
            .GreaterThan(0).WithMessage("ResourceId must be positive")
            .LessThan(int.MaxValue).WithMessage("ResourceId is too long");
        RuleFor(x => x.Unit).NotEmpty()
            .MaximumLength(10).WithMessage("{PropertyName} maximum length is 10 characters");
    }
}