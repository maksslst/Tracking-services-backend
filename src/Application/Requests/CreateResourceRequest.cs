using Domain.Enums;
using FluentValidation;

namespace Application.Requests;

public class CreateResourceRequest
{
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Source { get; set; } = null!;
    public int? CompanyId { get; set; }
    public ResourceStatus Status { get; set; } = ResourceStatus.Inactive;
}

public class CreateResourceRequestValidator : AbstractValidator<CreateResourceRequest>
{
    public CreateResourceRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty()
            .MaximumLength(ValidationConstants.ResourceNameLength)
            .WithMessage("{PropertyName} maximum length is 100 characters");
        RuleFor(x => x.Type).NotEmpty()
            .MaximumLength(ValidationConstants.ResourceTypeLength)
            .WithMessage("{PropertyName} maximum length is 100 characters");
        RuleFor(x => x.Source).NotEmpty()
            .MaximumLength(ValidationConstants.ResourceSourceLength)
            .WithMessage("{PropertyName} maximum length is 100 characters");
        RuleFor(x => x.CompanyId)
            .GreaterThan(0).WithMessage("CompanyId must be positive")
            .LessThan(int.MaxValue).WithMessage("CompanyId is too long");
        RuleFor(x => x.Status).NotNull();
    }
}