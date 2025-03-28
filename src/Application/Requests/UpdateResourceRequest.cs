using Domain.Enums;
using FluentValidation;

namespace Application.Requests;

public class UpdateResourceRequest
{
    public int ResourceId { get; set; }
    public int CompanyId { get; set; }
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Source { get; set; } = null!;
    public ResourceStatus Status { get; set; } = ResourceStatus.Inactive;
}

public class UpdateResourceRequestValidator : AbstractValidator<UpdateResourceRequest>
{
    public UpdateResourceRequestValidator()
    {
        RuleFor(x => x.ResourceId).NotNull()
            .GreaterThan(0).WithMessage("ResourceId must be positive");
        RuleFor(x => x.Name).NotEmpty()
            .MaximumLength(100).WithMessage("{PropertyName} maximum length is 100 characters");
        RuleFor(x => x.Type).NotEmpty()
            .MaximumLength(100).WithMessage("{PropertyName} maximum length is 100 characters");
        RuleFor(x => x.Source).NotEmpty()
            .MaximumLength(100).WithMessage("{PropertyName} maximum length is 100 characters");
        RuleFor(x => x.CompanyId)
            .GreaterThan(0).WithMessage("CompanyId must be positive")
            .LessThan(int.MaxValue).WithMessage("CompanyId is too long");
        RuleFor(x => x.Status).NotNull();
    }
}