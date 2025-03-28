using FluentValidation;

namespace Application.Requests;

public class UpdateMonitoringSettingRequest
{
    public int Id { get; set; }
    public int ResourceId { get; set; }
    public string CheckInterval { get; set; } = null!;
    public bool Mode { get; set; }
}

public class UpdateMonitoringSettingRequestValidator : AbstractValidator<UpdateMonitoringSettingRequest>
{
    public UpdateMonitoringSettingRequestValidator()
    {
        RuleFor(r => r.Id).NotNull()
            .GreaterThan(0).WithMessage("Id must be positive");
        RuleFor(x => x.ResourceId).NotNull()
            .GreaterThan(0).WithMessage("ResourceId must be positive")
            .LessThanOrEqualTo(100).WithMessage("ResourceId is too long");
        RuleFor(x => x.CheckInterval).NotEmpty()
            .MaximumLength(50).WithMessage("{PropertyName} maximum length is 50 characters");
        RuleFor(x => x.Mode).NotNull();
    }
}